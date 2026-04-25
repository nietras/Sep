```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7184/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     1.371 ms |  1.00 |  29 | 21288.2 |   27.4 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     1.511 ms |  1.10 |  29 | 19318.0 |   30.2 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     1.399 ms |  1.02 |  29 | 20858.2 |   28.0 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     1.911 ms |  1.39 |  29 | 15271.3 |   38.2 |       8.45 KB |        8.32 |
| ReadLine_    | Row   | 50000   |     7.884 ms |  5.75 |  29 |  3701.4 |  157.7 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Row   | 50000   |    23.786 ms | 17.35 |  29 |  1226.8 |  475.7 |      19.95 KB |       19.64 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Cols  | 50000   |     2.232 ms |  1.00 |  29 | 13076.0 |   44.6 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     2.369 ms |  1.06 |  29 | 12315.8 |   47.4 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     3.283 ms |  1.47 |  29 |  8889.4 |   65.7 |       8.46 KB |        8.32 |
| ReadLine_    | Cols  | 50000   |     8.279 ms |  3.71 |  29 |  3524.9 |  165.6 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Cols  | 50000   |    44.021 ms | 19.73 |  29 |   662.9 |  880.4 |     445.61 KB |      438.75 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 50000   |    22.918 ms |  1.00 |  29 |  1273.3 |  458.4 |    13802.3 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    15.072 ms |  0.66 |  29 |  1936.2 |  301.4 |   13997.81 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    25.305 ms |  1.10 |  29 |  1153.2 |  506.1 |   13962.14 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    80.250 ms |  3.50 |  29 |   363.6 | 1605.0 |  102133.88 KB |        7.40 |
| CsvHelper    | Asset | 50000   |    54.268 ms |  2.37 |  29 |   537.7 | 1085.4 |   13970.19 KB |        1.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 1000000 |   441.901 ms |  1.00 | 583 |  1321.1 |  441.9 |  266667.44 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   191.048 ms |  0.43 | 583 |  3055.7 |  191.0 |  267864.88 KB |        1.00 |
| Sylvan___    | Asset | 1000000 |   529.177 ms |  1.20 | 583 |  1103.2 |  529.2 |  266824.41 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,312.114 ms |  2.97 | 583 |   444.9 | 1312.1 | 2038834.94 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,160.448 ms |  2.63 | 583 |   503.1 | 1160.4 |  266840.68 KB |        1.00 |
