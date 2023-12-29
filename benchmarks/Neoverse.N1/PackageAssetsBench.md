```

BenchmarkDotNet v0.13.11, Ubuntu 22.04.2 LTS (Jammy Jellyfish)
Unknown processor
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  Job-EEMDRF : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD

Job=Job-EEMDRF  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=False  
Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    12.72 ms |  1.00 |  29 | 2287.5 |  254.3 |       1020 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    12.75 ms |  1.00 |  29 | 2281.2 |  255.0 |       1020 B |        1.00 |
| Sylvan___    | Row   | 50000   |    31.29 ms |  2.46 |  29 |  929.4 |  625.9 |       6269 B |        6.15 |
| ReadLine_    | Row   | 50000   |    37.12 ms |  2.92 |  29 |  783.5 |  742.5 |   90734916 B |   88,955.80 |
| CsvHelper    | Row   | 50000   |    91.34 ms |  7.18 |  29 |  318.4 | 1826.9 |      21272 B |       20.85 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    15.00 ms |  1.00 |  29 | 1938.6 |  300.1 |       1032 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    16.05 ms |  1.07 |  29 | 1811.8 |  321.1 |       1035 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    35.43 ms |  2.36 |  29 |  821.0 |  708.6 |       6288 B |        6.09 |
| ReadLine_    | Cols  | 50000   |    41.72 ms |  2.78 |  29 |  697.2 |  834.3 |   90734929 B |   87,921.44 |
| CsvHelper    | Cols  | 50000   |   137.46 ms |  9.16 |  29 |  211.6 | 2749.3 |     457144 B |      442.97 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    68.24 ms |  1.00 |  29 |  426.2 | 1364.9 |   14134756 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    38.78 ms |  0.57 |  29 |  750.1 |  775.5 |   14191876 B |        1.00 |
| Sylvan___    | Asset | 50000   |   100.82 ms |  1.48 |  29 |  288.5 | 2016.5 |   14295846 B |        1.01 |
| ReadLine_    | Asset | 50000   |   157.86 ms |  2.31 |  29 |  184.2 | 3157.2 |  104585308 B |        7.40 |
| CsvHelper    | Asset | 50000   |   167.11 ms |  2.43 |  29 |  174.1 | 3342.2 |   14309064 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 | 1,357.03 ms |  1.00 | 581 |  428.8 | 1357.0 |  273070824 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   648.57 ms |  0.48 | 581 |  897.2 |  648.6 |  277540480 B |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,990.02 ms |  1.47 | 581 |  292.4 | 1990.0 |  273234920 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,137.22 ms |  2.32 | 581 |  185.5 | 3137.2 | 2087767336 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 3,391.21 ms |  2.50 | 581 |  171.6 | 3391.2 |  273241296 B |        1.00 |
