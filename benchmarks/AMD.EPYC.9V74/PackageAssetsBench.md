```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32522/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______    | Row   | 50000   |     3.510 ms |  1.00 |  29 | 8314.1 |   70.2 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.974 ms |  1.13 |  29 | 7343.2 |   79.5 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.581 ms |  1.02 |  29 | 8147.9 |   71.6 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.654 ms |  1.33 |  29 | 6270.1 |   93.1 |       8.46 KB |        8.26 |
| ReadLine_    | Row   | 50000   |    18.775 ms |  5.35 |  29 | 1554.3 |  375.5 |   88608.23 KB |   86,579.03 |
| CsvHelper    | Row   | 50000   |    65.365 ms | 18.63 |  29 |  446.4 | 1307.3 |      20.02 KB |       19.56 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     5.223 ms |  1.00 |  29 | 5587.0 |  104.5 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.188 ms |  1.19 |  29 | 4715.9 |  123.8 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.899 ms |  1.71 |  29 | 3279.0 |  178.0 |       8.46 KB |        8.27 |
| ReadLine_    | Cols  | 50000   |    18.805 ms |  3.60 |  29 | 1551.8 |  376.1 |   88608.23 KB |   86,579.03 |
| CsvHelper    | Cols  | 50000   |   108.848 ms | 20.85 |  29 |  268.1 | 2177.0 |     445.67 KB |      435.47 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    44.330 ms |  1.00 |  29 |  658.3 |  886.6 |   13802.25 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    33.980 ms |  0.77 |  29 |  858.8 |  679.6 |   13873.15 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    49.397 ms |  1.11 |  29 |  590.7 |  987.9 |    13961.9 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   145.582 ms |  3.29 |  29 |  200.4 | 2911.6 |  102133.65 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   147.697 ms |  3.33 |  29 |  197.6 | 2953.9 |   13971.03 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   939.331 ms |  1.00 | 583 |  621.5 |  939.3 |  266667.11 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   614.165 ms |  0.65 | 583 |  950.5 |  614.2 |  276740.09 KB |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,115.543 ms |  1.19 | 583 |  523.3 | 1115.5 |  266829.55 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,948.147 ms |  3.14 | 583 |  198.0 | 2948.1 | 2038834.11 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,930.616 ms |  3.12 | 583 |  199.2 | 2930.6 |  266838.59 KB |        1.00 |
