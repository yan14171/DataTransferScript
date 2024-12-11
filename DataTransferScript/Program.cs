// Database connection string
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.VisualBasic;
using System.Diagnostics;
using Microsoft.Identity.Client;
using DataTransferScript.Models;

namespace DataTransferScript
{
    public static class DataTransfer
    {
        public static async Task Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args), "First argument should always provide a uri to csv file");
            }

            string csvFilePath = args[0];

            if (csvFilePath.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(args), "First argument should always provide a uri to csv file");
            }

            //connection string input is ommited as it is local and windows authentication to begin with
            string? connectionString = Environment.GetEnvironmentVariable("CabDataConnectionString");

            if (connectionString.IsNullOrEmpty())
            {
                //try and fall back to args
                connectionString = args[1];

                if (connectionString.IsNullOrEmpty())
                {
                    throw new ArgumentException("Neither arguments, nor Environment variables don't contain connection string to the database", nameof(args));
                }
            }

            var cabRides = GetCabRidesFromCSV(csvFilePath);

            await Console.Out.WriteLineAsync($"Read {cabRides.Count} entities from the file");

            var duplicates = GetDuplicates(cabRides);

            cabRides.RemoveAll(duplicates.Contains);

            await Console.Out.WriteLineAsync($"Removed {duplicates.Count} duplicates");

            await WriteCabRidesIntoCSV("duplicates.csv", duplicates);

            await Console.Out.WriteLineAsync($"Logged all of the duplicates into \"duplicates.csv\"");

            await BulkInsertionAsync(cabRides, connectionString);
            //await NaiveInsertionAsync(cabRides, connectionString);
        }

        private static List<CabRide> GetDuplicates(List<CabRide> cabRides) => cabRides
                .GroupBy(r => new { r.PickupDatetime, r.DropoffDatetime, r.PassengerCount })
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Skip(1))
                .ToList();

        private static async Task BulkInsertionAsync(IEnumerable<CabRide> records, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataTable dataTable = ConvertRecordsToDataTable(records);

                await BulkInsertDataAsync(dataTable, connection);
            }
        }

        private static async Task NaiveInsertionAsync(IEnumerable<CabRide> records, string connectionString)
        {
            var insertionTasks = new List<Task>(records.Count());

            foreach (var record in records)
            {
                insertionTasks.Add(InsertRecordAsync(record, connectionString));
            }

            await Task.WhenAll(insertionTasks);

            Console.WriteLine("Data upload complete.");
        }


        private static async Task InsertRecordAsync(CabRide record, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                await Console.Out.WriteLineAsync("Connection established succesfully");

                string query = @"
            INSERT INTO Cab_Rides (
                tpep_pickup_datetime, 
                tpep_dropoff_datetime, 
                passenger_count, 
                trip_distance, 
                store_and_fwd_flag, 
                PULocationID, 
                DOLocationID, 
                fare_amount, 
                tip_amount
            ) VALUES (
                @PickupDatetime, 
                @DropoffDatetime, 
                @PassengerCount, 
                @TripDistance, 
                @StoreAndFwdFlag, 
                @PULocationID, 
                @DOLocationID, 
                @FareAmount, 
                @TipAmount
            )";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PickupDatetime", record.PickupDatetime ?? default);
                    command.Parameters.AddWithValue("@DropoffDatetime", record.DropoffDatetime ?? default);
                    command.Parameters.AddWithValue("@PassengerCount", record.PassengerCount ?? 1);
                    command.Parameters.AddWithValue("@TripDistance", record.TripDistance ?? 0);
                    command.Parameters.AddWithValue("@StoreAndFwdFlag", record.StoreAndFwdFlag.IsNullOrEmpty() ? "NO" : record.StoreAndFwdFlag);
                    command.Parameters.AddWithValue("@PULocationID", record.PULocationID ?? 1);
                    command.Parameters.AddWithValue("@DOLocationID", record.DOLocationID ?? 1);
                    command.Parameters.AddWithValue("@FareAmount", record.FareAmount ?? 0);
                    command.Parameters.AddWithValue("@TipAmount", record.TipAmount ?? (object)DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }
                await connection.CloseAsync();
            }
        }

        private static async Task BulkInsertDataAsync(DataTable dataTable, SqlConnection connection)
        {
            await connection.OpenAsync();

            await Console.Out.WriteLineAsync("Connection established succesfully");

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = "Cab_Rides";
            
                bulkCopy.ColumnMappings.Add("tpep_pickup_datetime", "tpep_pickup_datetime");
                bulkCopy.ColumnMappings.Add("tpep_dropoff_datetime", "tpep_dropoff_datetime");
                bulkCopy.ColumnMappings.Add("passenger_count", "passenger_count");
                bulkCopy.ColumnMappings.Add("trip_distance", "trip_distance");
                bulkCopy.ColumnMappings.Add("store_and_fwd_flag", "store_and_fwd_flag");
                bulkCopy.ColumnMappings.Add("PULocationID", "PULocationID");
                bulkCopy.ColumnMappings.Add("DOLocationID", "DOLocationID");
                bulkCopy.ColumnMappings.Add("fare_amount", "fare_amount");
                bulkCopy.ColumnMappings.Add("tip_amount", "tip_amount");
            
                try
                {
                    await bulkCopy.WriteToServerAsync(dataTable);
                    Console.WriteLine("Data inserted successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during bulk insert: {ex.Message}");
                }
            }
        }


        private static List<CabRide> GetCabRidesFromCSV(string csvFilePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            };

            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<CabRideMap>();
                return csv.GetRecords<CabRide>()
                    .Select(n =>
                    {
                        n.CleanData();
                        n.ConvertToUtc();
                        return n;
                    }).ToList();
            }
        }

        private static async Task WriteCabRidesIntoCSV(string csvFilePath, IEnumerable<CabRide> records)
        {
            using (var writer = new StreamWriter(csvFilePath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)))
            {
                await csv.WriteRecordsAsync(records);
            }
        }

        private static DataTable ConvertRecordsToDataTable(IEnumerable<CabRide> records)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("tpep_pickup_datetime", typeof(DateTime));
            dt.Columns.Add("tpep_dropoff_datetime", typeof(DateTime));
            dt.Columns.Add("passenger_count", typeof(int));
            dt.Columns.Add("trip_distance", typeof(double));
            dt.Columns.Add("store_and_fwd_flag", typeof(string));
            dt.Columns.Add("PULocationID", typeof(int));
            dt.Columns.Add("DOLocationID", typeof(int));
            dt.Columns.Add("fare_amount", typeof(double));
            dt.Columns.Add("tip_amount", typeof(double));

            foreach (var record in records)
            {
                DataRow row = dt.NewRow();
                row["tpep_pickup_datetime"] = record.PickupDatetime ?? default;
                row["tpep_dropoff_datetime"] = record.DropoffDatetime ?? default;
                row["passenger_count"] = record.PassengerCount ?? 1;
                row["trip_distance"] = record.TripDistance ?? 0;
                row["store_and_fwd_flag"] = record.StoreAndFwdFlag.IsNullOrEmpty() ? "No" : record.StoreAndFwdFlag;
                row["PULocationID"] = record.PULocationID ?? 1;
                row["DOLocationID"] = record.DOLocationID ?? 1;
                row["fare_amount"] = record.FareAmount ?? 0;
                row["tip_amount"] = record.TipAmount ?? (object)DBNull.Value;
                dt.Rows.Add(row);
            }

            return dt;
        }
    }
}