```

BenchmarkDotNet v0.15.1, Linux Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-GLYBTL : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-GLYBTL  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    10.56 ms |  1.00 |  33 | 3151.1 |  211.2 |       1.06 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    10.56 ms |  1.00 |  33 | 3151.2 |  211.2 |       1.06 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.30 ms |  0.97 |  33 | 3232.1 |  205.9 |       1.06 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    23.32 ms |  2.21 |  33 | 1427.4 |  466.3 |       8.53 KB |        8.05 |
| ReadLine_    | Row   | 50000   |    25.52 ms |  2.42 |  33 | 1304.2 |  510.4 |  108779.66 KB |  102,663.94 |
| CsvHelper    | Row   | 50000   |    78.90 ms |  7.47 |  33 |  421.8 | 1577.9 |      20.21 KB |       19.07 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    11.86 ms |  1.00 |  33 | 2805.7 |  237.2 |       1.07 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.77 ms |  1.16 |  33 | 2416.7 |  275.4 |       1.08 KB |        1.01 |
| Sylvan___    | Cols  | 50000   |    27.03 ms |  2.28 |  33 | 1231.5 |  540.5 |       7.75 KB |        7.26 |
| ReadLine_    | Cols  | 50000   |    27.57 ms |  2.32 |  33 | 1207.3 |  551.3 |  108778.81 KB |  101,818.55 |
| CsvHelper    | Cols  | 50000   |   107.56 ms |  9.07 |  33 |  309.4 | 2151.1 |     448.95 KB |      420.22 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    47.34 ms |  1.00 |  33 |  703.0 |  946.9 |   13802.87 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    30.11 ms |  0.64 |  33 | 1105.4 |  602.2 |   13860.16 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    70.84 ms |  1.50 |  33 |  469.8 | 1416.9 |   13962.65 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   150.57 ms |  3.18 |  33 |  221.0 | 3011.5 |  122305.23 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   125.64 ms |  2.65 |  33 |  264.9 | 2512.9 |   13970.57 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   957.99 ms |  1.00 | 665 |  695.0 |  958.0 |  266670.54 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   567.65 ms |  0.59 | 665 | 1172.9 |  567.6 |   270791.1 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,426.53 ms |  1.49 | 665 |  466.7 | 1426.5 |  266828.12 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,099.98 ms |  3.24 | 665 |  214.8 | 3100.0 | 2442321.02 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,576.17 ms |  2.69 | 665 |  258.4 | 2576.2 |  266835.59 KB |        1.00 |
