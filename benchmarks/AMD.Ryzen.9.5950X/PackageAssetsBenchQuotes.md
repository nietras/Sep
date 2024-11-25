```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2
  Job-LKXTKX : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-LKXTKX  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     7.077 ms |  1.00 |  33 | 4716.4 |  141.5 |       1.04 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     6.916 ms |  0.98 |  33 | 4826.4 |  138.3 |       1.04 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    16.794 ms |  2.37 |  33 | 1987.5 |  335.9 |       7.69 KB |        7.42 |
| ReadLine_    | Row   | 50000   |    14.068 ms |  1.99 |  33 | 2372.5 |  281.4 |  108778.74 KB |  104,886.47 |
| CsvHelper    | Row   | 50000   |    52.336 ms |  7.40 |  33 |  637.7 | 1046.7 |      20.01 KB |       19.29 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     7.709 ms |  1.00 |  33 | 4329.4 |  154.2 |       1.03 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     8.991 ms |  1.17 |  33 | 3712.1 |  179.8 |       1.04 KB |        1.01 |
| Sylvan___    | Cols  | 50000   |    19.529 ms |  2.53 |  33 | 1709.2 |  390.6 |        7.7 KB |        7.47 |
| ReadLine_    | Cols  | 50000   |    15.205 ms |  1.97 |  33 | 2195.1 |  304.1 |  108778.76 KB |  105,482.43 |
| CsvHelper    | Cols  | 50000   |    70.755 ms |  9.18 |  33 |  471.7 | 1415.1 |     448.87 KB |      435.27 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    37.129 ms |  1.00 |  33 |  899.0 |  742.6 |   13803.03 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    21.253 ms |  0.57 |  33 | 1570.5 |  425.1 |   13981.42 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    48.990 ms |  1.32 |  33 |  681.3 |  979.8 |   13962.13 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   118.684 ms |  3.20 |  33 |  281.2 | 2373.7 |  122305.02 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    83.758 ms |  2.26 |  33 |  398.5 | 1675.2 |   13970.38 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   707.538 ms |  1.00 | 667 |  943.7 |  707.5 |  266668.99 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   349.710 ms |  0.49 | 667 | 1909.3 |  349.7 |  268243.91 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,022.358 ms |  1.44 | 667 |  653.1 | 1022.4 |  266825.07 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,210.209 ms |  3.12 | 667 |  302.1 | 2210.2 | 2442320.25 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,709.222 ms |  2.42 | 667 |  390.6 | 1709.2 |   266833.3 KB |        1.00 |
