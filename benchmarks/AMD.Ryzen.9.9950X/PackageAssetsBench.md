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
| Sep______    | Row   | 50000   |     1.385 ms |  1.00 |  29 | 21072.5 |   27.7 |       1.01 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     1.518 ms |  1.10 |  29 | 19225.7 |   30.4 |       1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     1.393 ms |  1.01 |  29 | 20942.6 |   27.9 |       1.01 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     1.885 ms |  1.36 |  29 | 15484.5 |   37.7 |       7.65 KB |        7.59 |
| ReadLine_    | Row   | 50000   |     7.885 ms |  5.69 |  29 |  3701.1 |  157.7 |   88608.23 KB |   87,921.34 |
| CsvHelper    | Row   | 50000   |    23.848 ms | 17.22 |  29 |  1223.6 |  477.0 |      19.95 KB |       19.79 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Cols  | 50000   |     2.010 ms |  1.00 |  29 | 14520.0 |   40.2 |       1.01 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     2.366 ms |  1.18 |  29 | 12332.1 |   47.3 |       1.01 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     3.188 ms |  1.59 |  29 |  9152.3 |   63.8 |       7.65 KB |        7.59 |
| ReadLine_    | Cols  | 50000   |     8.349 ms |  4.15 |  29 |  3495.3 |  167.0 |   88608.23 KB |   87,921.34 |
| CsvHelper    | Cols  | 50000   |    43.794 ms | 21.79 |  29 |   666.3 |  875.9 |     445.61 KB |      442.15 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 50000   |    24.029 ms |  1.00 |  29 |  1214.4 |  480.6 |   13802.35 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    14.474 ms |  0.60 |  29 |  2016.1 |  289.5 |    13994.5 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    25.166 ms |  1.05 |  29 |  1159.6 |  503.3 |   13962.14 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    78.997 ms |  3.29 |  29 |   369.4 | 1579.9 |  102133.85 KB |        7.40 |
| CsvHelper    | Asset | 50000   |    54.192 ms |  2.26 |  29 |   538.5 | 1083.8 |   13973.22 KB |        1.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 1000000 |   441.913 ms |  1.00 | 583 |  1321.0 |  441.9 |  266667.27 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   186.231 ms |  0.42 | 583 |  3134.7 |  186.2 |  268775.45 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   520.745 ms |  1.18 | 583 |  1121.1 |  520.7 |  266824.38 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,377.806 ms |  3.12 | 583 |   423.7 | 1377.8 | 2038834.84 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,173.256 ms |  2.65 | 583 |   497.6 | 1173.3 |  266840.63 KB |        1.00 |
