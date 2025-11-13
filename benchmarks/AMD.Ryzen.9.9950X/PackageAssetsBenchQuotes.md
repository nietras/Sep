```

BenchmarkDotNet v0.15.6, Windows 10 (10.0.19045.6575/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     4.288 ms |  1.00 |  33 | 7783.5 |   85.8 |       1.01 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     5.226 ms |  1.22 |  33 | 6386.4 |  104.5 |       1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.170 ms |  0.97 |  33 | 8004.3 |   83.4 |       1.15 KB |        1.14 |
| Sylvan___    | Row   | 50000   |    10.286 ms |  2.40 |  33 | 3245.1 |  205.7 |       7.65 KB |        7.59 |
| ReadLine_    | Row   | 50000   |     9.464 ms |  2.21 |  33 | 3526.9 |  189.3 |  108778.73 KB |  107,935.48 |
| CsvHelper    | Row   | 50000   |    27.086 ms |  6.32 |  33 | 1232.3 |  541.7 |      19.95 KB |       19.79 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     5.100 ms |  1.00 |  33 | 6544.9 |  102.0 |       1.01 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.496 ms |  1.08 |  33 | 6073.5 |  109.9 |       1.01 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    11.899 ms |  2.33 |  33 | 2805.1 |  238.0 |       7.66 KB |        7.60 |
| ReadLine_    | Cols  | 50000   |    10.023 ms |  1.97 |  33 | 3330.1 |  200.5 |  108778.73 KB |  107,935.48 |
| CsvHelper    | Cols  | 50000   |    40.244 ms |  7.89 |  33 |  829.4 |  804.9 |     445.61 KB |      442.15 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    28.037 ms |  1.00 |  33 | 1190.5 |  560.7 |    13802.3 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    17.037 ms |  0.61 |  33 | 1959.1 |  340.7 |   13989.51 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    31.020 ms |  1.11 |  33 | 1076.0 |  620.4 |   13962.02 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    95.065 ms |  3.39 |  33 |  351.1 | 1901.3 |  122304.75 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    51.339 ms |  1.83 |  33 |  650.1 | 1026.8 |   13970.75 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   519.607 ms |  1.00 | 667 | 1285.0 |  519.6 |  266667.37 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   221.654 ms |  0.43 | 667 | 3012.4 |  221.7 |   270630.1 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   703.908 ms |  1.35 | 667 |  948.6 |  703.9 |  266824.38 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,870.691 ms |  3.60 | 667 |  356.9 | 1870.7 | 2442318.59 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,097.942 ms |  2.11 | 667 |  608.1 | 1097.9 |  266832.53 KB |        1.00 |
