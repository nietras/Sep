```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.86GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    11.14 ms |  1.00 |  33 | 2986.8 |  222.9 |      1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    10.09 ms |  0.91 |  33 | 3299.8 |  201.7 |      1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.78 ms |  0.97 |  33 | 3088.0 |  215.6 |      1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    24.19 ms |  2.17 |  33 | 1375.8 |  483.8 |      8.47 KB |        8.34 |
| ReadLine_    | Row   | 50000   |    25.76 ms |  2.31 |  33 | 1291.8 |  515.3 | 108778.73 KB |  107,105.21 |
| CsvHelper    | Row   | 50000   |    71.55 ms |  6.42 |  33 |  465.2 | 1431.0 |     20.02 KB |       19.71 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    12.56 ms |  1.00 |  33 | 2649.3 |  251.2 |      1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.16 ms |  1.05 |  33 | 2530.0 |  263.1 |      1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    28.48 ms |  2.27 |  33 | 1168.6 |  569.6 |      8.47 KB |        8.34 |
| ReadLine_    | Cols  | 50000   |    28.04 ms |  2.23 |  33 | 1186.8 |  560.8 | 108778.73 KB |  107,105.21 |
| CsvHelper    | Cols  | 50000   |   102.93 ms |  8.19 |  33 |  323.3 | 2058.7 |    445.68 KB |      438.82 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    47.13 ms |  1.00 |  33 |  706.2 |  942.6 |   13802.8 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    35.51 ms |  0.75 |  33 |  937.2 |  710.3 |  13861.34 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    70.49 ms |  1.50 |  33 |  472.1 | 1409.8 |  13961.98 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   166.63 ms |  3.54 |  33 |  199.7 | 3332.7 | 122304.53 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   126.46 ms |  2.68 |  33 |  263.2 | 2529.2 |  13971.99 KB |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   996.03 ms |  1.00 | 665 |  668.5 |  996.0 | 266667.33 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   632.62 ms |  0.64 | 665 | 1052.5 |  632.6 | 273042.72 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,460.60 ms |  1.47 | 665 |  455.8 | 1460.6 | 266824.24 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 4,404.24 ms |  4.42 | 665 |  151.2 | 4404.2 | 2442317.7 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,565.79 ms |  2.58 | 665 |  259.5 | 2565.8 | 266839.62 KB |        1.00 |
