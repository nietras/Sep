```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.33 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     3.378 ms |  1.00 |  29 | 8609.4 |   67.6 |      1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.574 ms |  1.06 |  29 | 8138.2 |   71.5 |      1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.555 ms |  1.05 |  29 | 8182.1 |   71.1 |      1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.484 ms |  1.33 |  29 | 6486.0 |   89.7 |      8.46 KB |        8.33 |
| ReadLine_    | Row   | 50000   |    20.972 ms |  6.21 |  29 | 1386.9 |  419.4 |  88608.23 KB |   87,245.02 |
| CsvHelper    | Row   | 50000   |    63.706 ms | 18.86 |  29 |  456.6 | 1274.1 |     19.95 KB |       19.64 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     4.807 ms |  1.00 |  29 | 6051.1 |   96.1 |      1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.929 ms |  1.23 |  29 | 4905.7 |  118.6 |      1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     7.895 ms |  1.64 |  29 | 3684.1 |  157.9 |      8.46 KB |        8.33 |
| ReadLine_    | Cols  | 50000   |    21.111 ms |  4.39 |  29 | 1377.8 |  422.2 |  88608.23 KB |   87,245.02 |
| CsvHelper    | Cols  | 50000   |   106.644 ms | 22.19 |  29 |  272.7 | 2132.9 |     445.6 KB |      438.75 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    44.960 ms |  1.00 |  29 |  646.9 |  899.2 |  13802.46 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    28.930 ms |  0.64 |  29 | 1005.4 |  578.6 |  13871.64 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    50.912 ms |  1.13 |  29 |  571.3 | 1018.2 |  13961.95 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   139.178 ms |  3.10 |  29 |  209.0 | 2783.6 | 102133.57 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   124.336 ms |  2.77 |  29 |  233.9 | 2486.7 |  13969.95 KB |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   879.828 ms |  1.00 | 581 |  661.4 |  879.8 | 266668.04 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   543.031 ms |  0.62 | 581 | 1071.5 |  543.0 | 275573.26 KB |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,069.606 ms |  1.22 | 581 |  544.0 | 1069.6 | 266824.96 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,096.154 ms |  3.52 | 581 |  187.9 | 3096.2 | 2038836.3 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,627.515 ms |  2.99 | 581 |  221.5 | 2627.5 | 266834.49 KB |        1.00 |
