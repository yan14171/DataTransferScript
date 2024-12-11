To simplify the structure of this script, I used Microsoft.Data.SqlClient for its ease of use and speed of setup.
If needed, logging can be abstracted behind Abstractions.Logging package's capabilities
I didn't really do any perf testing of the code, by sqlbulk is surely quicker

**Further optimisations*
    *Use something like [csvkit](https://csvkit.readthedocs.io/en/latest/index.html) to work with large csv files
    *If the dataset is very large, maybe partitioning by tpep_pickup_datetime to optimize queries over time ranges.
    *Additional column time_spent can be added, to create a descending index for querying 100 longest fares in terms of time spent traveling
    *It would take additional time, but probably by utilizing CsvHelper's IAsyncEnumerable we can make NaiveInsertion faster by shoving the logic of 
        reading values from SQL into one of the `insertionTasks`
    *Duplication removal part can be probably optimized. I would take my time to read into such problems
