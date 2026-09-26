```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 3.08GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.38 GB Available
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
| Sep______    | Row   | 50000   |    10.36 ms |  1.00 |  33 | 3211.8 |  207.3 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    10.30 ms |  0.99 |  33 | 3230.4 |  206.1 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.18 ms |  0.98 |  33 | 3269.9 |  203.6 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    24.89 ms |  2.40 |  33 | 1337.0 |  497.9 |       8.47 KB |        8.34 |
| ReadLine_    | Row   | 50000   |    25.91 ms |  2.50 |  33 | 1284.7 |  518.1 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Row   | 50000   |    77.45 ms |  7.47 |  33 |  429.7 | 1548.9 |      19.95 KB |       19.64 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    11.66 ms |  1.00 |  33 | 2853.6 |  233.3 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.05 ms |  1.12 |  33 | 2550.6 |  261.0 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    28.12 ms |  2.41 |  33 | 1183.6 |  562.4 |       8.47 KB |        8.34 |
| ReadLine_    | Cols  | 50000   |    25.28 ms |  2.17 |  33 | 1316.4 |  505.6 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Cols  | 50000   |   104.61 ms |  8.97 |  33 |  318.2 | 2092.1 |      445.6 KB |      438.75 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    50.67 ms |  1.00 |  33 |  656.8 | 1013.4 |   13802.27 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    32.65 ms |  0.64 |  33 | 1019.5 |  652.9 |   13870.18 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    70.27 ms |  1.39 |  33 |  473.7 | 1405.3 |   13962.58 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   162.45 ms |  3.21 |  33 |  204.9 | 3249.0 |  122304.82 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   124.21 ms |  2.45 |  33 |  267.9 | 2484.2 |   13969.95 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   968.41 ms |  1.00 | 665 |  687.5 |  968.4 |   266667.9 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   594.56 ms |  0.61 | 665 | 1119.8 |  594.6 |  272414.32 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,408.40 ms |  1.45 | 665 |  472.7 | 1408.4 |   266828.8 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,341.03 ms |  3.45 | 665 |  199.3 | 3341.0 | 2442320.23 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,614.00 ms |  2.70 | 665 |  254.7 | 2614.0 |  266839.16 KB |        1.00 |
