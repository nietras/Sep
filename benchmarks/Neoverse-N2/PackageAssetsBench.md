```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     5.764 ms |  1.00 |  29 | 5045.8 |  115.3 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     6.044 ms |  1.05 |  29 | 4812.1 |  120.9 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     5.844 ms |  1.01 |  29 | 4977.3 |  116.9 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    20.859 ms |  3.62 |  29 | 1394.4 |  417.2 |       7516 B |        7.83 |
| ReadLine_    | Row   | 50000   |    22.921 ms |  3.98 |  29 | 1268.9 |  458.4 |   90734824 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    54.659 ms |  9.49 |  29 |  532.1 | 1093.2 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     7.400 ms |  1.00 |  29 | 3930.7 |  148.0 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     8.230 ms |  1.11 |  29 | 3534.3 |  164.6 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    25.446 ms |  3.44 |  29 | 1143.0 |  508.9 |       7516 B |        7.83 |
| ReadLine_    | Cols  | 50000   |    23.095 ms |  3.12 |  29 | 1259.4 |  461.9 |   90734824 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    87.315 ms | 11.80 |  29 |  333.1 | 1746.3 |     456368 B |      475.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    32.159 ms |  1.00 |  29 |  904.4 |  643.2 |   14133000 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    14.713 ms |  0.46 |  29 | 1976.8 |  294.3 |   14184435 B |        1.00 |
| Sylvan___    | Asset | 50000   |    57.448 ms |  1.79 |  29 |  506.3 | 1149.0 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   127.374 ms |  3.96 |  29 |  228.4 | 2547.5 |  104584112 B |        7.40 |
| CsvHelper    | Asset | 50000   |    99.985 ms |  3.11 |  29 |  290.9 | 1999.7 |   14305310 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   795.463 ms |  1.00 | 581 |  731.5 |  795.5 |  273063672 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   362.114 ms |  0.46 | 581 | 1606.9 |  362.1 |  282418904 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,251.563 ms |  1.57 | 581 |  464.9 | 1251.6 |  273225344 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,910.094 ms |  3.66 | 581 |  200.0 | 2910.1 | 2087773496 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,233.116 ms |  2.81 | 581 |  260.6 | 2233.1 |  273234792 B |        1.00 |
