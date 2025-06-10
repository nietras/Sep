```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-FXNRMG : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-FXNRMG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     9.740 ms |  1.00 |  33 | 3426.9 |  194.8 |        995 B |        1.00 |
| Sep_Async    | Row   | 50000   |    10.119 ms |  1.04 |  33 | 3298.6 |  202.4 |        954 B |        0.96 |
| Sep_Unescape | Row   | 50000   |    10.247 ms |  1.05 |  33 | 3257.2 |  204.9 |        976 B |        0.98 |
| Sylvan___    | Row   | 50000   |    23.525 ms |  2.42 |  33 | 1418.8 |  470.5 |       6683 B |        6.72 |
| ReadLine_    | Row   | 50000   |    30.016 ms |  3.08 |  33 | 1112.0 |  600.3 |  111389426 B |  111,949.17 |
| CsvHelper    | Row   | 50000   |    63.934 ms |  6.56 |  33 |  522.1 | 1278.7 |      20512 B |       20.62 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    10.978 ms |  1.00 |  33 | 3040.4 |  219.6 |        954 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    12.836 ms |  1.17 |  33 | 2600.3 |  256.7 |        954 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    29.183 ms |  2.66 |  33 | 1143.7 |  583.7 |       6874 B |        7.21 |
| ReadLine_    | Cols  | 50000   |    30.967 ms |  2.82 |  33 | 1077.8 |  619.3 |  111389422 B |  116,760.40 |
| CsvHelper    | Cols  | 50000   |    98.644 ms |  8.99 |  33 |  338.4 | 1972.9 |     456312 B |      478.31 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    49.729 ms |  1.00 |  33 |  671.2 |  994.6 |   14133888 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    28.551 ms |  0.58 |  33 | 1169.0 |  571.0 |   14193244 B |        1.00 |
| Sylvan___    | Asset | 50000   |    70.563 ms |  1.42 |  33 |  473.0 | 1411.3 |   14296674 B |        1.01 |
| ReadLine_    | Asset | 50000   |   165.448 ms |  3.34 |  33 |  201.7 | 3309.0 |  125240196 B |        8.86 |
| CsvHelper    | Asset | 50000   |   117.307 ms |  2.37 |  33 |  284.5 | 2346.1 |   14306942 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 | 1,027.672 ms |  1.00 | 667 |  649.7 | 1027.7 |  273072096 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   469.985 ms |  0.46 | 667 | 1420.7 |  470.0 |  279329376 B |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,439.859 ms |  1.40 | 667 |  463.7 | 1439.9 |  273227688 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,952.028 ms |  2.87 | 667 |  226.2 | 2952.0 | 2500934744 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,354.660 ms |  2.29 | 667 |  283.6 | 2354.7 |  273241544 B |        1.00 |
