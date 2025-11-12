```

BenchmarkDotNet v0.15.6, Windows 10 (10.0.19045.6575/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     1.386 ms |  1.00 |  29 | 21052.1 |   27.7 |       1.01 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     1.538 ms |  1.11 |  29 | 18977.3 |   30.8 |       1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     1.417 ms |  1.02 |  29 | 20592.3 |   28.3 |       1.01 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     1.928 ms |  1.39 |  29 | 15138.2 |   38.6 |       7.65 KB |        7.59 |
| ReadLine_    | Row   | 50000   |     7.982 ms |  5.76 |  29 |  3655.7 |  159.6 |   88608.23 KB |   87,921.34 |
| CsvHelper    | Row   | 50000   |    24.110 ms | 17.39 |  29 |  1210.3 |  482.2 |      19.95 KB |       19.79 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Cols  | 50000   |     2.032 ms |  1.00 |  29 | 14360.9 |   40.6 |       1.01 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     2.403 ms |  1.18 |  29 | 12141.4 |   48.1 |       1.01 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     3.218 ms |  1.58 |  29 |  9068.2 |   64.4 |       7.65 KB |        7.59 |
| ReadLine_    | Cols  | 50000   |     8.332 ms |  4.10 |  29 |  3502.4 |  166.6 |   88608.23 KB |   87,921.34 |
| CsvHelper    | Cols  | 50000   |    44.458 ms | 21.88 |  29 |   656.4 |  889.2 |     445.61 KB |      442.15 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 50000   |    24.072 ms |  1.00 |  29 |  1212.3 |  481.4 |   13802.35 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    14.762 ms |  0.61 |  29 |  1976.8 |  295.2 |   13995.84 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    25.171 ms |  1.05 |  29 |  1159.3 |  503.4 |   13962.18 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    80.894 ms |  3.36 |  29 |   360.7 | 1617.9 |  102133.88 KB |        7.40 |
| CsvHelper    | Asset | 50000   |    54.442 ms |  2.26 |  29 |   536.0 | 1088.8 |   13971.39 KB |        1.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 1000000 |   455.076 ms |  1.00 | 583 |  1282.8 |  455.1 |  266667.38 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   186.676 ms |  0.41 | 583 |  3127.3 |  186.7 |  268533.73 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   521.421 ms |  1.15 | 583 |  1119.6 |  521.4 |  266824.48 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,309.915 ms |  2.88 | 583 |   445.7 | 1309.9 | 2038834.41 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,160.782 ms |  2.55 | 583 |   502.9 | 1160.8 |  266840.46 KB |        1.00 |
