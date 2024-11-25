```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-RAOLFZ : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-RAOLFZ  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    10.54 ms |  1.00 |  33 | 3159.0 |  210.7 |       1.06 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.80 ms |  1.02 |  33 | 3083.0 |  215.9 |       1.06 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    26.19 ms |  2.49 |  33 | 1270.9 |  523.7 |       7.74 KB |        7.30 |
| ReadLine_    | Row   | 50000   |    28.51 ms |  2.71 |  33 | 1167.2 |  570.3 |  108778.83 KB |  102,568.62 |
| CsvHelper    | Row   | 50000   |    76.92 ms |  7.30 |  33 |  432.7 | 1538.3 |      20.28 KB |       19.12 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    11.63 ms |  1.00 |  33 | 2860.6 |  232.7 |       1.07 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.58 ms |  1.17 |  33 | 2450.1 |  271.7 |       1.08 KB |        1.01 |
| Sylvan___    | Cols  | 50000   |    30.23 ms |  2.60 |  33 | 1100.8 |  604.7 |       7.76 KB |        7.25 |
| ReadLine_    | Cols  | 50000   |    29.64 ms |  2.55 |  33 | 1122.7 |  592.9 |  108778.84 KB |  101,540.13 |
| CsvHelper    | Cols  | 50000   |   105.52 ms |  9.07 |  33 |  315.4 | 2110.3 |     445.86 KB |      416.19 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    38.31 ms |  1.00 |  33 |  868.8 |  766.1 |   13802.34 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    26.27 ms |  0.69 |  33 | 1266.9 |  525.4 |    13864.9 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    62.31 ms |  1.63 |  33 |  534.1 | 1246.3 |   13964.99 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   138.98 ms |  3.63 |  33 |  239.5 | 2779.6 |  122303.89 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   117.01 ms |  3.05 |  33 |  284.4 | 2340.3 |   13970.29 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   958.41 ms |  1.00 | 665 |  694.7 |  958.4 |  266669.05 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   609.74 ms |  0.64 | 665 | 1091.9 |  609.7 |  269433.56 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,452.68 ms |  1.52 | 665 |  458.3 | 1452.7 |  266827.02 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,888.60 ms |  4.06 | 665 |  171.2 | 3888.6 | 2442326.16 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,539.78 ms |  2.65 | 665 |  262.1 | 2539.8 |  266834.94 KB |        1.00 |
