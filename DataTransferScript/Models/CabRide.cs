// Database connection string
namespace DataTransferScript.Models
{
    class CabRide
    {
        public DateTime? PickupDatetime { get; set; } = default;
        public DateTime? DropoffDatetime { get; set; } = default;
        public int? PassengerCount { get; set; } = 1;
        public double? TripDistance { get; set; } = 0;
        public string StoreAndFwdFlag { get; set; } = "N";
        public int? PULocationID { get; set; } = 1;
        public int? DOLocationID { get; set; } = 1;
        public double? FareAmount { get; set; } = 0;
        public double? TipAmount { get; set; }

        public void CleanData()
        {
            if (StoreAndFwdFlag != null)
            {
                StoreAndFwdFlag = StoreAndFwdFlag.Trim().ToUpper() switch
                {
                    "N" => "No",
                    "Y" => "Yes",
                    _ => "No"
                };
            }
        }

        public void ConvertToUtc()
        {
            TimeZoneInfo estTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            if (PickupDatetime.HasValue)
            {
                try
                {
                    PickupDatetime = TimeZoneInfo.ConvertTimeToUtc(PickupDatetime.Value, estTimeZone);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error converting PickupDatetime to UTC: {ex.Message}");
                    PickupDatetime = null;
                }
            }

            if (DropoffDatetime.HasValue)
            {
                try
                {
                    DropoffDatetime = TimeZoneInfo.ConvertTimeToUtc(DropoffDatetime.Value, estTimeZone);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error converting DropoffDatetime to UTC: {ex.Message}");
                    DropoffDatetime = null;
                }
            }
        }

        public void ValidateAndSanitize()
        {
            if (PassengerCount.HasValue && PassengerCount.Value <= 0)
            {
                Console.WriteLine($"Invalid PassengerCount: {PassengerCount}. Setting to 1.");
                PassengerCount = 1;
            }

            if (TripDistance.HasValue && TripDistance.Value < 0)
            {
                Console.WriteLine($"Invalid TripDistance: {TripDistance}. Setting to 0.");
                TripDistance = 0;
            }

            if (FareAmount.HasValue && FareAmount.Value < 0)
            {
                Console.WriteLine($"Invalid FareAmount: {FareAmount}. Setting to 0.");
                FareAmount = 0;
            }

            if (TipAmount.HasValue && TipAmount.Value < 0)
            {
                Console.WriteLine($"Invalid TipAmount: {TipAmount}. Setting to null.");
                TipAmount = null;
            }
        }
    }
}