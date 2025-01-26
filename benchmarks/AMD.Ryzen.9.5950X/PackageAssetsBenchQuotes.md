```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2
  Job-WRHRFC : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2

Job=Job-WRHRFC  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     7.397 ms |  1.00 |  33 | 4512.4 |  147.9 |       1.04 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     7.432 ms |  1.00 |  33 | 4490.8 |  148.6 |       1.04 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     7.093 ms |  0.96 |  33 | 4706.0 |  141.9 |       1.04 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    17.571 ms |  2.38 |  33 | 1899.5 |  351.4 |       7.69 KB |        7.41 |
| ReadLine_    | Row   | 50000   |    14.336 ms |  1.94 |  33 | 2328.2 |  286.7 |  108778.75 KB |  104,689.33 |
| CsvHelper    | Row   | 50000   |    52.672 ms |  7.12 |  33 |  633.7 | 1053.4 |      20.05 KB |       19.29 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     8.126 ms |  1.00 |  33 | 4107.5 |  162.5 |       1.04 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     9.748 ms |  1.20 |  33 | 3424.0 |  195.0 |       1.05 KB |        1.01 |
| Sylvan___    | Cols  | 50000   |    20.503 ms |  2.52 |  33 | 1628.0 |  410.1 |        7.7 KB |        7.39 |
| ReadLine_    | Cols  | 50000   |    16.513 ms |  2.03 |  33 | 2021.3 |  330.3 |  108778.76 KB |  104,394.99 |
| CsvHelper    | Cols  | 50000   |    74.224 ms |  9.13 |  33 |  449.7 | 1484.5 |     445.85 KB |      427.88 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    39.523 ms |  1.00 |  33 |  844.5 |  790.5 |   13802.63 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    23.386 ms |  0.59 |  33 | 1427.2 |  467.7 |   13981.76 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    50.803 ms |  1.29 |  33 |  657.0 | 1016.1 |   13962.08 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   114.306 ms |  2.89 |  33 |  292.0 | 2286.1 |  122304.45 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    88.786 ms |  2.25 |  33 |  375.9 | 1775.7 |   13970.43 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   752.681 ms |  1.00 | 667 |  887.1 |  752.7 |     266669 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   377.733 ms |  0.50 | 667 | 1767.7 |  377.7 |   267992.5 KB |        1.00 |
| Sylvan___    | Asset | 1000000 | 1,091.345 ms |  1.45 | 667 |  611.8 | 1091.3 |  266825.09 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,615.390 ms |  3.47 | 667 |  255.3 | 2615.4 | 2442319.06 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,756.409 ms |  2.33 | 667 |  380.2 | 1756.4 |  266839.53 KB |        1.00 |
