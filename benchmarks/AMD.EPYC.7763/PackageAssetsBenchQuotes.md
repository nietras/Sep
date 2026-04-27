```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 3.24GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______    | Row   | 50000   |    11.31 ms |  1.00 |  33 | 2943.7 |  226.1 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    10.85 ms |  0.96 |  33 | 3066.4 |  217.1 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.88 ms |  0.96 |  33 | 3058.0 |  217.7 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    24.41 ms |  2.16 |  33 | 1363.7 |  488.1 |       8.47 KB |        8.32 |
| ReadLine_    | Row   | 50000   |    26.31 ms |  2.33 |  33 | 1265.2 |  526.1 |  108778.73 KB |  106,899.63 |
| CsvHelper    | Row   | 50000   |    77.51 ms |  6.86 |  33 |  429.4 | 1550.1 |      20.02 KB |       19.67 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    12.68 ms |  1.00 |  33 | 2624.0 |  253.7 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.03 ms |  1.03 |  33 | 2553.4 |  260.7 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    28.13 ms |  2.22 |  33 | 1183.1 |  562.6 |       8.47 KB |        8.34 |
| ReadLine_    | Cols  | 50000   |    27.09 ms |  2.14 |  33 | 1228.4 |  541.9 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Cols  | 50000   |   105.53 ms |  8.32 |  33 |  315.4 | 2110.7 |     445.67 KB |      438.82 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    45.70 ms |  1.00 |  33 |  728.2 |  914.1 |    13802.2 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    31.67 ms |  0.69 |  33 | 1051.0 |  633.3 |   13860.96 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    68.56 ms |  1.50 |  33 |  485.4 | 1371.3 |   13961.93 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   154.44 ms |  3.38 |  33 |  215.5 | 3088.7 |  122304.28 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   127.01 ms |  2.78 |  33 |  262.0 | 2540.2 |   13971.03 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   964.21 ms |  1.00 | 665 |  690.5 |  964.2 |  266672.62 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   581.44 ms |  0.60 | 665 | 1145.1 |  581.4 |  272279.71 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,373.94 ms |  1.42 | 665 |  484.6 | 1373.9 |  266827.98 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,241.30 ms |  3.36 | 665 |  205.4 | 3241.3 | 2442318.33 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,568.45 ms |  2.66 | 665 |  259.2 | 2568.5 |  266835.41 KB |        1.00 |
