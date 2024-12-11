// Database connection string
using CsvHelper.Configuration;

namespace DataTransferScript.Models
{
    class CabRideMap : ClassMap<CabRide>
    {
        public CabRideMap()
        {
            Map(m => m.PickupDatetime).Name("tpep_pickup_datetime");
            Map(m => m.DropoffDatetime).Name("tpep_dropoff_datetime");
            Map(m => m.PassengerCount).Name("passenger_count").Optional();
            Map(m => m.TripDistance).Name("trip_distance");
            Map(m => m.StoreAndFwdFlag).Name("store_and_fwd_flag");
            Map(m => m.PULocationID).Name("PULocationID");
            Map(m => m.DOLocationID).Name("DOLocationID");
            Map(m => m.FareAmount).Name("fare_amount");
            Map(m => m.TipAmount).Name("tip_amount").Optional();
        }
    }
}