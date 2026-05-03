```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.71GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    11.51 ms |  1.00 |  33 | 2890.8 |  230.3 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    10.93 ms |  0.95 |  33 | 3044.2 |  218.7 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    11.11 ms |  0.97 |  33 | 2995.1 |  222.2 |        1.4 KB |        1.37 |
| Sylvan___    | Row   | 50000   |    24.78 ms |  2.15 |  33 | 1343.0 |  495.6 |       8.47 KB |        8.27 |
| ReadLine_    | Row   | 50000   |    29.40 ms |  2.55 |  33 | 1132.0 |  588.0 |  108778.73 KB |  106,287.61 |
| CsvHelper    | Row   | 50000   |    78.17 ms |  6.79 |  33 |  425.8 | 1563.4 |      19.95 KB |       19.49 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    12.90 ms |  1.00 |  33 | 2580.4 |  258.0 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.33 ms |  1.03 |  33 | 2496.3 |  266.6 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    28.40 ms |  2.20 |  33 | 1172.0 |  567.9 |       8.47 KB |        8.28 |
| ReadLine_    | Cols  | 50000   |    28.83 ms |  2.24 |  33 | 1154.5 |  576.6 |  108778.73 KB |  106,287.61 |
| CsvHelper    | Cols  | 50000   |   107.25 ms |  8.32 |  33 |  310.3 | 2145.0 |     445.67 KB |      435.47 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    46.22 ms |  1.00 |  33 |  720.1 |  924.3 |   13802.78 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    30.47 ms |  0.66 |  33 | 1092.5 |  609.3 |   13861.96 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    69.49 ms |  1.50 |  33 |  479.0 | 1389.7 |   13961.93 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   177.24 ms |  3.84 |  33 |  187.8 | 3544.9 |  122304.56 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   126.26 ms |  2.73 |  33 |  263.6 | 2525.2 |    13970.8 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,004.23 ms |  1.00 | 665 |  663.0 | 1004.2 |  266672.12 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   604.21 ms |  0.60 | 665 | 1101.9 |  604.2 |  271818.09 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,405.74 ms |  1.40 | 665 |  473.6 | 1405.7 |   266824.2 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,753.31 ms |  3.74 | 665 |  177.4 | 3753.3 | 2442317.95 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,592.07 ms |  2.58 | 665 |  256.9 | 2592.1 |  266840.93 KB |        1.00 |
