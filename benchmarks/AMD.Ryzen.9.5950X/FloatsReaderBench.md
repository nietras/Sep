```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2
  Job-WRHRFC : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.013 ms |  1.00 | 20 | 10093.4 |   80.5 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   2.355 ms |  1.17 | 20 |  8627.4 |   94.2 |     10.7 KB |        8.56 |
| ReadLine_ | Row    | 25000 |   9.787 ms |  4.86 | 20 |  2076.1 |  391.5 | 73489.63 KB |   58,791.71 |
| CsvHelper | Row    | 25000 |  25.143 ms | 12.49 | 20 |   808.2 | 1005.7 |       20 KB |       16.00 |
|           |        |       |            |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |   2.666 ms |  1.00 | 20 |  7622.2 |  106.6 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   3.702 ms |  1.39 | 20 |  5488.4 |  148.1 |    10.71 KB |        8.54 |
| ReadLine_ | Cols   | 25000 |  10.544 ms |  3.96 | 20 |  1927.1 |  421.8 | 73489.63 KB |   58,654.23 |
| CsvHelper | Cols   | 25000 |  27.442 ms | 10.29 | 20 |   740.5 | 1097.7 | 21340.34 KB |   17,032.36 |
|           |        |       |            |       |    |         |        |             |             |
| Sep______ | Floats | 25000 |  20.297 ms |  1.00 | 20 |  1001.1 |  811.9 |     7.97 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   3.780 ms |  0.19 | 20 |  5375.6 |  151.2 |   179.49 KB |       22.51 |
| Sylvan___ | Floats | 25000 |  52.343 ms |  2.58 | 20 |   388.2 | 2093.7 |    18.88 KB |        2.37 |
| ReadLine_ | Floats | 25000 |  68.698 ms |  3.38 | 20 |   295.8 | 2747.9 | 73493.12 KB |    9,215.89 |
| CsvHelper | Floats | 25000 | 100.913 ms |  4.97 | 20 |   201.4 | 4036.5 | 22061.69 KB |    2,766.49 |
