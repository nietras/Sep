```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.82GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.424 ms |  1.00 |  29 | 8494.4 |   68.5 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.705 ms |  1.08 |  29 | 7850.6 |   74.1 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.446 ms |  1.01 |  29 | 8441.3 |   68.9 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.282 ms |  1.25 |  29 | 6792.3 |   85.6 |       8.46 KB |        8.33 |
| ReadLine_    | Row   | 50000   |    21.993 ms |  6.42 |  29 | 1322.5 |  439.9 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Row   | 50000   |    66.703 ms | 19.48 |  29 |  436.1 | 1334.1 |      20.02 KB |       19.71 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.779 ms |  1.00 |  29 | 6086.3 |   95.6 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.737 ms |  1.20 |  29 | 5070.1 |  114.7 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.312 ms |  1.74 |  29 | 3499.2 |  166.2 |       8.46 KB |        8.33 |
| ReadLine_    | Cols  | 50000   |    23.421 ms |  4.90 |  29 | 1241.9 |  468.4 |   88608.23 KB |   87,245.03 |
| CsvHelper    | Cols  | 50000   |   102.703 ms | 21.49 |  29 |  283.2 | 2054.1 |     445.67 KB |      438.82 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    39.161 ms |  1.00 |  29 |  742.7 |  783.2 |   13802.21 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    28.187 ms |  0.72 |  29 | 1031.9 |  563.7 |   13868.27 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    47.958 ms |  1.23 |  29 |  606.5 |  959.2 |   13961.98 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   126.257 ms |  3.23 |  29 |  230.4 | 2525.1 |  102133.95 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   122.942 ms |  3.14 |  29 |  236.6 | 2458.8 |   13970.21 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   813.625 ms |  1.00 | 581 |  715.2 |  813.6 |  266667.16 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   499.384 ms |  0.61 | 581 | 1165.2 |  499.4 |  276101.18 KB |        1.04 |
| Sylvan___    | Asset | 1000000 |   981.216 ms |  1.21 | 581 |  593.0 |  981.2 |  266824.05 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,494.903 ms |  3.07 | 581 |  233.2 | 2494.9 | 2038842.26 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,484.231 ms |  3.05 | 581 |  234.2 | 2484.2 |  266843.14 KB |        1.00 |
