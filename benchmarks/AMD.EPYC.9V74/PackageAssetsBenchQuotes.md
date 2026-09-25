```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.34 GB Available
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
| Sep______    | Row   | 50000   |    10.01 ms |  1.00 |  33 | 3323.8 |  200.3 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    10.04 ms |  1.00 |  33 | 3315.1 |  200.8 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.46 ms |  1.05 |  33 | 3181.2 |  209.2 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    24.91 ms |  2.49 |  33 | 1336.0 |  498.2 |       8.47 KB |        8.34 |
| ReadLine_    | Row   | 50000   |    25.08 ms |  2.51 |  33 | 1326.8 |  501.7 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Row   | 50000   |    71.64 ms |  7.16 |  33 |  464.6 | 1432.9 |      19.95 KB |       19.64 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    11.29 ms |  1.00 |  33 | 2948.2 |  225.8 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.32 ms |  1.18 |  33 | 2498.6 |  266.4 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    28.54 ms |  2.53 |  33 | 1166.0 |  570.9 |       8.47 KB |        8.34 |
| ReadLine_    | Cols  | 50000   |    25.37 ms |  2.25 |  33 | 1311.7 |  507.5 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Cols  | 50000   |   101.29 ms |  8.97 |  33 |  328.6 | 2025.8 |      445.6 KB |      438.75 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    52.20 ms |  1.00 |  33 |  637.6 | 1043.9 |   13802.12 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    34.50 ms |  0.66 |  33 |  964.7 |  690.0 |   13863.24 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    73.32 ms |  1.40 |  33 |  453.9 | 1466.5 |   13961.95 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   188.38 ms |  3.61 |  33 |  176.7 | 3767.6 |  122304.68 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   120.73 ms |  2.31 |  33 |  275.7 | 2414.6 |   13969.95 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,027.63 ms |  1.00 | 665 |  647.9 | 1027.6 |   266668.2 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   631.13 ms |  0.61 | 665 | 1054.9 |  631.1 |  272092.97 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,496.95 ms |  1.46 | 665 |  444.8 | 1497.0 |  266825.16 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,860.35 ms |  3.76 | 665 |  172.5 | 3860.4 | 2442320.57 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,557.50 ms |  2.49 | 665 |  260.3 | 2557.5 |  266834.37 KB |        1.00 |
