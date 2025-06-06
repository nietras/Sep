```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-MPBGVI : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-MPBGVI  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    10.36 ms |  1.00 |  33 | 3212.6 |  207.2 |       1.06 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    10.87 ms |  1.05 |  33 | 3062.0 |  217.4 |       1.06 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.93 ms |  1.05 |  33 | 3046.2 |  218.5 |       1.06 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    23.10 ms |  2.23 |  33 | 1440.8 |  462.0 |       7.73 KB |        7.30 |
| ReadLine_    | Row   | 50000   |    28.01 ms |  2.70 |  33 | 1188.1 |  560.3 |  108778.81 KB |  102,663.13 |
| CsvHelper    | Row   | 50000   |    78.83 ms |  7.61 |  33 |  422.2 | 1576.6 |      20.28 KB |       19.14 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    11.66 ms |  1.00 |  33 | 2855.2 |  233.1 |       1.07 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.20 ms |  1.13 |  33 | 2520.8 |  264.1 |       1.08 KB |        1.01 |
| Sylvan___    | Cols  | 50000   |    27.13 ms |  2.33 |  33 | 1226.8 |  542.6 |       7.75 KB |        7.26 |
| ReadLine_    | Cols  | 50000   |    28.43 ms |  2.44 |  33 | 1170.6 |  568.6 |  108778.82 KB |  101,818.56 |
| CsvHelper    | Cols  | 50000   |   106.43 ms |  9.13 |  33 |  312.7 | 2128.5 |     445.94 KB |      417.41 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    47.49 ms |  1.00 |  33 |  700.9 |  949.7 |   13802.88 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    31.48 ms |  0.66 |  33 | 1057.1 |  629.7 |   13860.54 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    69.08 ms |  1.46 |  33 |  481.8 | 1381.7 |   13963.14 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   145.48 ms |  3.06 |  33 |  228.8 | 2909.6 |  122305.22 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   126.76 ms |  2.67 |  33 |  262.6 | 2535.1 |   13971.05 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,000.40 ms |  1.00 | 665 |  665.5 | 1000.4 |  266670.06 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   571.52 ms |  0.57 | 665 | 1165.0 |  571.5 |  272741.85 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,434.88 ms |  1.43 | 665 |  464.0 | 1434.9 |  266826.78 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,039.15 ms |  3.04 | 665 |  219.1 | 3039.2 | 2442332.76 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,581.28 ms |  2.58 | 665 |  257.9 | 2581.3 |  266837.21 KB |        1.00 |
