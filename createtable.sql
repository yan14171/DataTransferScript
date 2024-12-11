CREATE TABLE Cab_Rides (
	TripId INT IDENTITY(1,1) PRIMARY KEY, --Not mentioned in the specs, but overall better than using a combined PK
    tpep_pickup_datetime DATETIME NOT NULL,
    tpep_dropoff_datetime DATETIME NOT NULL,
	--if furher optimization is needed, additional column time_spent can be added, to create a descending index for querying 100 longest fares in terms of time spent traveling
    passenger_count TINYINT DEFAULT 1, --Setting as 1
    trip_distance FLOAT NOT NULL,
    store_and_fwd_flag CHAR(3) DEFAULT 'NO',
    PULocationID INT NOT NULL,
    DOLocationID INT NOT NULL,
    fare_amount FLOAT NOT NULL,
    tip_amount FLOAT DEFAULT 0.0 --Defaults set up in place to prevent lost data from potentially unsafe source
);

--indexes to optimise traffic intensive queries from the task

CREATE INDEX IX_TaxiTrips_PULocationID_TipAmount
ON Cab_Rides (PULocationID, tip_amount);

CREATE INDEX IX_TaxiTrips_TripDistance
ON Cab_Rides (trip_distance DESC);

CREATE INDEX IX_TaxiTrips_TimeSpent
ON Cab_Rides (tpep_pickup_datetime, tpep_dropoff_datetime);

CREATE INDEX IX_TaxiTrips_PULocationID
ON Cab_Rides (PULocationID);