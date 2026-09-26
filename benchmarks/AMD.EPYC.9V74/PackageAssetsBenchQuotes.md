```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.33 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    10.12 ms |  1.00 |  33 | 3290.2 |  202.3 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    10.06 ms |  0.99 |  33 | 3308.8 |  201.2 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.00 ms |  0.99 |  33 | 3327.6 |  200.0 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    24.04 ms |  2.38 |  33 | 1384.4 |  480.8 |       8.47 KB |        8.34 |
| ReadLine_    | Row   | 50000   |    24.25 ms |  2.40 |  33 | 1372.5 |  485.0 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Row   | 50000   |    71.83 ms |  7.10 |  33 |  463.4 | 1436.5 |      19.95 KB |       19.64 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    11.23 ms |  1.00 |  33 | 2964.4 |  224.5 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.30 ms |  1.18 |  33 | 2503.1 |  265.9 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    27.93 ms |  2.49 |  33 | 1191.6 |  558.6 |       8.47 KB |        8.34 |
| ReadLine_    | Cols  | 50000   |    25.76 ms |  2.29 |  33 | 1292.1 |  515.1 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Cols  | 50000   |   101.46 ms |  9.04 |  33 |  328.0 | 2029.1 |      445.6 KB |      438.75 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    52.10 ms |  1.00 |  33 |  638.8 | 1042.0 |   13802.57 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    34.04 ms |  0.65 |  33 |  977.8 |  680.7 |   13862.08 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    73.37 ms |  1.41 |  33 |  453.6 | 1467.5 |   13961.96 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   184.81 ms |  3.55 |  33 |  180.1 | 3696.2 |   122304.5 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   120.11 ms |  2.31 |  33 |  277.1 | 2402.2 |   13969.95 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,002.06 ms |  1.00 | 665 |  664.4 | 1002.1 |  266668.01 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   637.22 ms |  0.64 | 665 | 1044.9 |  637.2 |  271708.08 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,456.85 ms |  1.45 | 665 |  457.0 | 1456.9 |  266827.98 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,799.84 ms |  3.79 | 665 |  175.2 | 3799.8 | 2442318.72 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,541.18 ms |  2.54 | 665 |  262.0 | 2541.2 |  266844.72 KB |        1.00 |
