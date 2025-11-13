```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26100.6899/24H2/2024Update/HudsonValley)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics 3.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.501 ms |  1.00 |  29 | 8334.3 |   70.0 |       1.01 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.748 ms |  1.07 |  29 | 7785.1 |   75.0 |       1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.489 ms |  1.00 |  29 | 8364.7 |   69.8 |       1.01 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.436 ms |  1.27 |  29 | 6578.6 |   88.7 |       7.65 KB |        7.59 |
| ReadLine_    | Row   | 50000   |    18.126 ms |  5.18 |  29 | 1609.9 |  362.5 |   88608.23 KB |   87,921.34 |
| CsvHelper    | Row   | 50000   |    65.367 ms | 18.69 |  29 |  446.4 | 1307.3 |      19.95 KB |       19.79 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.800 ms |  1.00 |  29 | 6079.7 |   96.0 |       1.01 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.952 ms |  1.24 |  29 | 4902.5 |  119.0 |       1.01 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.572 ms |  1.79 |  29 | 3404.3 |  171.4 |       7.65 KB |        7.59 |
| ReadLine_    | Cols  | 50000   |    19.182 ms |  4.00 |  29 | 1521.3 |  383.6 |   88608.23 KB |   87,921.34 |
| CsvHelper    | Cols  | 50000   |   108.414 ms | 22.59 |  29 |  269.2 | 2168.3 |      445.6 KB |      442.15 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    52.283 ms |  1.00 |  29 |  558.1 | 1045.7 |   13802.65 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    34.441 ms |  0.66 |  29 |  847.3 |  688.8 |   13913.24 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    53.960 ms |  1.04 |  29 |  540.8 | 1079.2 |   13962.29 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   210.735 ms |  4.04 |  29 |  138.5 | 4214.7 |  102133.96 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   131.644 ms |  2.53 |  29 |  221.7 | 2632.9 |   13970.83 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   978.280 ms |  1.00 | 583 |  596.7 |  978.3 |  266668.88 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   515.844 ms |  0.53 | 583 | 1131.7 |  515.8 |  267776.55 KB |        1.00 |
| Sylvan___    | Asset | 1000000 | 1,130.594 ms |  1.16 | 583 |  516.4 | 1130.6 |  266825.13 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,155.567 ms |  3.23 | 583 |  185.0 | 3155.6 | 2038835.23 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,669.767 ms |  2.73 | 583 |  218.7 | 2669.8 |  266844.99 KB |        1.00 |
