```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3)
Cobalt 100, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-MYYDFG : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-MYYDFG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     9.870 ms |  1.00 |  33 | 3381.5 |  197.4 |        995 B |        1.00 |
| Sep_Async    | Row   | 50000   |    10.154 ms |  1.03 |  33 | 3287.1 |  203.1 |        986 B |        0.99 |
| Sep_Unescape | Row   | 50000   |    10.137 ms |  1.03 |  33 | 3292.5 |  202.7 |        987 B |        0.99 |
| Sylvan___    | Row   | 50000   |    23.537 ms |  2.39 |  33 | 1418.1 |  470.7 |       6683 B |        6.72 |
| ReadLine_    | Row   | 50000   |    30.022 ms |  3.04 |  33 | 1111.8 |  600.4 |  111389483 B |  111,949.23 |
| CsvHelper    | Row   | 50000   |    62.677 ms |  6.35 |  33 |  532.5 | 1253.5 |      20499 B |       20.60 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    11.294 ms |  1.00 |  33 | 2955.3 |  225.9 |        991 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    12.715 ms |  1.13 |  33 | 2625.0 |  254.3 |        997 B |        1.01 |
| Sylvan___    | Cols  | 50000   |    27.614 ms |  2.45 |  33 | 1208.7 |  552.3 |       6718 B |        6.78 |
| ReadLine_    | Cols  | 50000   |    31.215 ms |  2.76 |  33 | 1069.3 |  624.3 |  111389452 B |  112,401.06 |
| CsvHelper    | Cols  | 50000   |    97.546 ms |  8.64 |  33 |  342.2 | 1950.9 |     459474 B |      463.65 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    49.015 ms |  1.00 |  33 |  681.0 |  980.3 |   14134790 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    27.824 ms |  0.57 |  33 | 1199.6 |  556.5 |   14193094 B |        1.00 |
| Sylvan___    | Asset | 50000   |    69.940 ms |  1.43 |  33 |  477.2 | 1398.8 |   14296142 B |        1.01 |
| ReadLine_    | Asset | 50000   |   161.138 ms |  3.29 |  33 |  207.1 | 3222.8 |  125240028 B |        8.86 |
| CsvHelper    | Asset | 50000   |   118.262 ms |  2.42 |  33 |  282.2 | 2365.2 |   14306036 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 | 1,029.915 ms |  1.00 | 667 |  648.3 | 1029.9 |  273069712 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   466.132 ms |  0.45 | 667 | 1432.4 |  466.1 |  280587560 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,444.270 ms |  1.40 | 667 |  462.3 | 1444.3 |  273229408 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,915.108 ms |  2.83 | 667 |  229.1 | 2915.1 | 2500933736 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,384.759 ms |  2.32 | 667 |  280.0 | 2384.8 |  273249888 B |        1.00 |
