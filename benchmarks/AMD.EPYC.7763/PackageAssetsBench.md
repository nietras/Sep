```

BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763 2.90GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.531 ms |  1.00 |  29 | 8236.4 |   70.6 |       1.01 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.778 ms |  1.07 |  29 | 7698.2 |   75.6 |       1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.520 ms |  1.00 |  29 | 8263.0 |   70.4 |       1.13 KB |        1.12 |
| Sylvan___    | Row   | 50000   |     4.298 ms |  1.22 |  29 | 6766.8 |   86.0 |       7.65 KB |        7.59 |
| ReadLine_    | Row   | 50000   |    21.890 ms |  6.20 |  29 | 1328.7 |  437.8 |   88608.23 KB |   87,921.34 |
| CsvHelper    | Row   | 50000   |    64.318 ms | 18.22 |  29 |  452.2 | 1286.4 |      19.95 KB |       19.79 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.921 ms |  1.00 |  29 | 5911.1 |   98.4 |       1.01 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.793 ms |  1.18 |  29 | 5020.8 |  115.9 |       1.01 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.024 ms |  1.63 |  29 | 3625.1 |  160.5 |       7.65 KB |        7.59 |
| ReadLine_    | Cols  | 50000   |    23.332 ms |  4.74 |  29 | 1246.6 |  466.6 |   88608.23 KB |   87,921.34 |
| CsvHelper    | Cols  | 50000   |   101.802 ms | 20.69 |  29 |  285.7 | 2036.0 |      445.6 KB |      442.15 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    39.836 ms |  1.00 |  29 |  730.1 |  796.7 |   13803.05 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    29.563 ms |  0.74 |  29 |  983.9 |  591.3 |   13875.06 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    47.912 ms |  1.20 |  29 |  607.1 |  958.2 |   13962.03 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   131.577 ms |  3.31 |  29 |  221.1 | 2631.5 |  102133.63 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   122.181 ms |  3.07 |  29 |  238.1 | 2443.6 |   13971.42 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   864.069 ms |  1.00 | 581 |  673.4 |  864.1 |  266667.16 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   516.021 ms |  0.60 | 581 | 1127.6 |  516.0 |  276325.84 KB |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,011.286 ms |  1.17 | 581 |  575.4 | 1011.3 |  266824.48 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,850.125 ms |  3.30 | 581 |  204.2 | 2850.1 | 2038834.76 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,579.496 ms |  2.99 | 581 |  225.6 | 2579.5 |  266840.54 KB |        1.00 |
