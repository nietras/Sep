```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-NMHWMW : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-NMHWMW  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    10.74 ms |  1.00 |  33 | 3099.5 |  214.8 |       1.06 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.84 ms |  1.01 |  33 | 3070.8 |  216.8 |       1.06 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    26.52 ms |  2.47 |  33 | 1254.7 |  530.5 |       7.74 KB |        7.29 |
| ReadLine_    | Row   | 50000   |    28.70 ms |  2.67 |  33 | 1159.5 |  574.1 |  108778.83 KB |  102,380.08 |
| CsvHelper    | Row   | 50000   |    78.31 ms |  7.29 |  33 |  425.0 | 1566.2 |      20.28 KB |       19.08 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    11.67 ms |  1.00 |  33 | 2850.9 |  233.5 |       1.07 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.64 ms |  1.17 |  33 | 2440.1 |  272.8 |       1.56 KB |        1.46 |
| Sylvan___    | Cols  | 50000   |    30.27 ms |  2.59 |  33 | 1099.4 |  605.5 |       7.76 KB |        7.27 |
| ReadLine_    | Cols  | 50000   |    30.08 ms |  2.58 |  33 | 1106.5 |  601.6 |  108778.83 KB |  101,818.58 |
| CsvHelper    | Cols  | 50000   |   106.00 ms |  9.08 |  33 |  314.0 | 2120.0 |     445.94 KB |      417.41 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    37.96 ms |  1.00 |  33 |  876.7 |  759.2 |   13802.34 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    31.49 ms |  0.83 |  33 | 1056.9 |  629.8 |   13856.23 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    63.27 ms |  1.67 |  33 |  526.1 | 1265.3 |   13961.98 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    96.59 ms |  2.54 |  33 |  344.6 | 1931.8 |   122303.6 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   116.46 ms |  3.07 |  33 |  285.8 | 2329.3 |   13970.29 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   963.99 ms |  1.00 | 665 |  690.7 |  964.0 |  266669.05 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   605.06 ms |  0.63 | 665 | 1100.4 |  605.1 |  269916.45 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,485.34 ms |  1.54 | 665 |  448.2 | 1485.3 |  266827.02 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,913.66 ms |  4.06 | 665 |  170.1 | 3913.7 | 2442327.47 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,511.80 ms |  2.61 | 665 |  265.1 | 2511.8 |  266834.99 KB |        1.00 |
