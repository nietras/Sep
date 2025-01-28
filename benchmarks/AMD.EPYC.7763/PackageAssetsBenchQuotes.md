```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.1 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-XDFYGT : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-XDFYGT  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    10.55 ms |  1.00 |  33 | 3154.2 |  211.0 |      1.06 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    12.02 ms |  1.14 |  33 | 2768.6 |  240.4 |      1.06 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.44 ms |  0.99 |  33 | 3189.2 |  208.7 |      1.06 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    25.83 ms |  2.45 |  33 | 1288.3 |  516.7 |      7.74 KB |        7.30 |
| ReadLine_    | Row   | 50000   |    25.94 ms |  2.46 |  33 | 1282.8 |  518.9 | 108778.82 KB |  102,568.61 |
| CsvHelper    | Row   | 50000   |    77.25 ms |  7.32 |  33 |  430.8 | 1545.0 |     20.29 KB |       19.13 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    12.01 ms |  1.00 |  33 | 2770.2 |  240.3 |      1.07 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.29 ms |  1.11 |  33 | 2505.2 |  265.7 |      1.08 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    29.56 ms |  2.46 |  33 | 1126.0 |  591.2 |      7.76 KB |        7.24 |
| ReadLine_    | Cols  | 50000   |    27.61 ms |  2.30 |  33 | 1205.6 |  552.1 | 108778.82 KB |  101,447.64 |
| CsvHelper    | Cols  | 50000   |   107.02 ms |  8.91 |  33 |  311.0 | 2140.3 |    445.93 KB |      415.88 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    47.85 ms |  1.00 |  33 |  695.6 |  956.9 |  13802.84 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    30.16 ms |  0.63 |  33 | 1103.4 |  603.2 |  13862.06 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    72.55 ms |  1.52 |  33 |  458.8 | 1450.9 |  13963.14 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   155.62 ms |  3.25 |  33 |  213.9 | 3112.3 | 122305.53 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   126.34 ms |  2.64 |  33 |  263.4 | 2526.8 |  13973.89 KB |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   986.97 ms |  1.00 | 665 |  674.6 |  987.0 | 266670.24 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   587.82 ms |  0.60 | 665 | 1132.7 |  587.8 | 272038.75 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,464.19 ms |  1.48 | 665 |  454.7 | 1464.2 | 266840.84 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,142.01 ms |  3.18 | 665 |  211.9 | 3142.0 | 2442321.3 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,609.30 ms |  2.64 | 665 |  255.2 | 2609.3 | 266834.41 KB |        1.00 |
