```

BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763 2.90GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    10.81 ms |  1.00 |  33 | 3079.3 |  216.2 |       1.01 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    11.24 ms |  1.04 |  33 | 2962.1 |  224.7 |       1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    11.45 ms |  1.06 |  33 | 2906.6 |  229.0 |       1.01 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    25.19 ms |  2.33 |  33 | 1321.3 |  503.8 |       7.66 KB |        7.60 |
| ReadLine_    | Row   | 50000   |    26.33 ms |  2.44 |  33 | 1263.9 |  526.6 |  108778.73 KB |  107,935.48 |
| CsvHelper    | Row   | 50000   |    78.33 ms |  7.25 |  33 |  424.9 | 1566.6 |      20.02 KB |       19.86 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    13.13 ms |  1.00 |  33 | 2535.3 |  262.5 |       1.01 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.97 ms |  1.06 |  33 | 2382.3 |  279.4 |       1.01 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    28.21 ms |  2.15 |  33 | 1179.7 |  564.2 |       7.67 KB |        7.61 |
| ReadLine_    | Cols  | 50000   |    27.63 ms |  2.10 |  33 | 1204.5 |  552.6 |  108778.73 KB |  107,935.48 |
| CsvHelper    | Cols  | 50000   |   106.12 ms |  8.08 |  33 |  313.6 | 2122.4 |     445.68 KB |      442.22 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    46.78 ms |  1.00 |  33 |  711.5 |  935.6 |   13802.41 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    30.32 ms |  0.65 |  33 | 1097.6 |  606.5 |   13869.65 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    69.34 ms |  1.48 |  33 |  480.0 | 1386.7 |   13962.04 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   150.28 ms |  3.21 |  33 |  221.5 | 3005.6 |  122304.69 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   126.16 ms |  2.70 |  33 |  263.8 | 2523.2 |   13971.27 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   973.20 ms |  1.00 | 665 |  684.1 |  973.2 |  266667.22 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   589.97 ms |  0.61 | 665 | 1128.5 |  590.0 |  271570.43 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,404.92 ms |  1.44 | 665 |  473.9 | 1404.9 |  266824.36 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,386.92 ms |  3.48 | 665 |  196.6 | 3386.9 | 2442318.06 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,553.57 ms |  2.62 | 665 |  260.7 | 2553.6 |  266834.87 KB |        1.00 |
