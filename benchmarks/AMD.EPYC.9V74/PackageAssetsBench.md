```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.34 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.309 ms |  1.00 |  29 | 8788.7 |   66.2 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.434 ms |  1.04 |  29 | 8469.0 |   68.7 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.447 ms |  1.04 |  29 | 8438.7 |   68.9 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.419 ms |  1.34 |  29 | 6582.0 |   88.4 |       8.46 KB |        8.33 |
| ReadLine_    | Row   | 50000   |    20.622 ms |  6.23 |  29 | 1410.4 |  412.4 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Row   | 50000   |    63.806 ms | 19.29 |  29 |  455.8 | 1276.1 |      19.95 KB |       19.64 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.753 ms |  1.00 |  29 | 6119.2 |   95.1 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.873 ms |  1.24 |  29 | 4952.7 |  117.5 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     7.966 ms |  1.68 |  29 | 3651.2 |  159.3 |       8.46 KB |        8.33 |
| ReadLine_    | Cols  | 50000   |    21.437 ms |  4.51 |  29 | 1356.8 |  428.7 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Cols  | 50000   |   107.016 ms | 22.52 |  29 |  271.8 | 2140.3 |      445.6 KB |      438.75 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    45.343 ms |  1.00 |  29 |  641.5 |  906.9 |    13802.8 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    29.278 ms |  0.65 |  29 |  993.4 |  585.6 |   13869.72 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    51.452 ms |  1.14 |  29 |  565.3 | 1029.0 |   13963.07 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   142.153 ms |  3.14 |  29 |  204.6 | 2843.1 |  102133.57 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   125.593 ms |  2.77 |  29 |  231.6 | 2511.9 |   13969.95 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   885.937 ms |  1.00 | 581 |  656.8 |  885.9 |  266668.03 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   564.349 ms |  0.64 | 581 | 1031.1 |  564.3 |  275679.32 KB |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,068.202 ms |  1.21 | 581 |  544.7 | 1068.2 |  266824.98 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,153.626 ms |  3.56 | 581 |  184.5 | 3153.6 | 2038836.41 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,640.459 ms |  2.98 | 581 |  220.4 | 2640.5 |   266839.4 KB |        1.00 |
