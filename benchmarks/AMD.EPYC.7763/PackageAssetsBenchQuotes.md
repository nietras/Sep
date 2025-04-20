```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-DRDGJI : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-DRDGJI  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    10.37 ms |  1.00 |  33 | 3209.0 |  207.4 |       1.06 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    11.98 ms |  1.16 |  33 | 2777.9 |  239.6 |       1.07 KB |        1.01 |
| Sep_Unescape | Row   | 50000   |    10.80 ms |  1.04 |  33 | 3082.6 |  215.9 |       1.06 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    26.61 ms |  2.57 |  33 | 1250.5 |  532.3 |       8.67 KB |        8.18 |
| ReadLine_    | Row   | 50000   |    26.19 ms |  2.53 |  33 | 1270.7 |  523.8 |  108778.81 KB |  102,663.13 |
| CsvHelper    | Row   | 50000   |    78.40 ms |  7.56 |  33 |  424.5 | 1568.0 |      20.28 KB |       19.14 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    11.72 ms |  1.00 |  33 | 2839.2 |  234.4 |       1.07 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.11 ms |  1.12 |  33 | 2539.4 |  262.1 |       1.07 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    30.91 ms |  2.64 |  33 | 1076.7 |  618.2 |       7.76 KB |        7.27 |
| ReadLine_    | Cols  | 50000   |    27.63 ms |  2.36 |  33 | 1204.5 |  552.6 |  108778.81 KB |  101,818.55 |
| CsvHelper    | Cols  | 50000   |   105.61 ms |  9.01 |  33 |  315.1 | 2112.3 |     445.94 KB |      417.41 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    47.56 ms |  1.00 |  33 |  699.8 |  951.1 |   13802.86 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    31.99 ms |  0.67 |  33 | 1040.4 |  639.8 |   13863.12 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    73.27 ms |  1.54 |  33 |  454.2 | 1465.5 |   13962.34 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   157.87 ms |  3.32 |  33 |  210.8 | 3157.4 |  122305.39 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   125.36 ms |  2.64 |  33 |  265.5 | 2507.2 |   13971.01 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   978.16 ms |  1.00 | 665 |  680.7 |  978.2 |  266670.47 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   570.02 ms |  0.58 | 665 | 1168.0 |  570.0 |  273627.52 KB |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,484.26 ms |  1.52 | 665 |  448.6 | 1484.3 |  266826.25 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,104.63 ms |  3.17 | 665 |  214.5 | 3104.6 | 2442321.76 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,556.98 ms |  2.61 | 665 |  260.4 | 2557.0 |  266839.98 KB |        1.00 |
