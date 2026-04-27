```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 3.24GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______    | Row   | 50000   |     3.560 ms |  1.00 |  29 | 8170.2 |   71.2 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.828 ms |  1.08 |  29 | 7598.4 |   76.6 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.621 ms |  1.02 |  29 | 8032.0 |   72.4 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.429 ms |  1.24 |  29 | 6566.5 |   88.6 |       8.46 KB |        8.33 |
| ReadLine_    | Row   | 50000   |    22.395 ms |  6.29 |  29 | 1298.7 |  447.9 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Row   | 50000   |    63.768 ms | 17.91 |  29 |  456.1 | 1275.4 |      19.95 KB |       19.64 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.975 ms |  1.00 |  29 | 5845.8 |   99.5 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.914 ms |  1.19 |  29 | 4917.8 |  118.3 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.336 ms |  1.68 |  29 | 3489.0 |  166.7 |       8.46 KB |        8.32 |
| ReadLine_    | Cols  | 50000   |    23.563 ms |  4.74 |  29 | 1234.4 |  471.3 |   88608.23 KB |   87,161.21 |
| CsvHelper    | Cols  | 50000   |   103.984 ms | 20.90 |  29 |  279.7 | 2079.7 |     445.67 KB |      438.39 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    39.992 ms |  1.00 |  29 |  727.3 |  799.8 |   13802.19 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    27.508 ms |  0.69 |  29 | 1057.4 |  550.2 |   13862.58 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    48.015 ms |  1.20 |  29 |  605.8 |  960.3 |    13961.9 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   123.265 ms |  3.08 |  29 |  236.0 | 2465.3 |  102133.82 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   122.541 ms |  3.07 |  29 |  237.4 | 2450.8 |    13970.6 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   856.853 ms |  1.00 | 581 |  679.1 |  856.9 |  266667.16 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   505.409 ms |  0.59 | 581 | 1151.3 |  505.4 |  275675.86 KB |        1.03 |
| Sylvan___    | Asset | 1000000 |   993.500 ms |  1.16 | 581 |  585.7 |  993.5 |   266824.3 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,714.306 ms |  3.17 | 581 |  214.4 | 2714.3 | 2038834.82 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,491.534 ms |  2.91 | 581 |  233.5 | 2491.5 |  266832.59 KB |        1.00 |
