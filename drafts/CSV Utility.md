
# CSVSimplex

The idea is to provide a simple decorator for basic CSV file functionality. A lot of game design, specially balancing and writing, happens in spreadsheets. Reading and writing from them should be simple and quick to do.

I found a very simple and easy to use library, [CsvUtil](https://github.com/sinbad/UnityCsvUtil), which works really well. I'm basically just wrapping the library to hide it's implementation, keeping exposed just the "read list" and "write list" functionalities. If there is need to any other more specific funcitonailities, the library should be extended directly.