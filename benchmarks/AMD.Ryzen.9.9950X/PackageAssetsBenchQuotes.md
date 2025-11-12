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
| Sep______    | Row   | 50000   |     4.246 ms |  1.00 |  33 | 7860.8 |   84.9 |       1.01 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     4.440 ms |  1.05 |  33 | 7517.9 |   88.8 |       1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.225 ms |  1.00 |  33 | 7900.7 |   84.5 |       1.01 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    10.530 ms |  2.48 |  33 | 3169.7 |  210.6 |       7.65 KB |        7.59 |
| ReadLine_    | Row   | 50000   |     9.514 ms |  2.24 |  33 | 3508.1 |  190.3 |  108778.73 KB |  107,935.48 |
| CsvHelper    | Row   | 50000   |    27.305 ms |  6.43 |  33 | 1222.4 |  546.1 |      19.95 KB |       19.79 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     5.135 ms |  1.00 |  33 | 6500.4 |  102.7 |       1.01 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.549 ms |  1.08 |  33 | 6015.4 |  111.0 |       1.01 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    11.843 ms |  2.31 |  33 | 2818.4 |  236.9 |       7.66 KB |        7.60 |
| ReadLine_    | Cols  | 50000   |    10.036 ms |  1.95 |  33 | 3325.8 |  200.7 |  108778.73 KB |  107,935.48 |
| CsvHelper    | Cols  | 50000   |    41.857 ms |  8.15 |  33 |  797.4 |  837.1 |     445.61 KB |      442.15 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    27.018 ms |  1.00 |  33 | 1235.4 |  540.4 |   13802.35 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    15.694 ms |  0.58 |  33 | 2126.7 |  313.9 |   13995.08 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    32.032 ms |  1.19 |  33 | 1042.0 |  640.6 |   13962.01 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    95.125 ms |  3.52 |  33 |  350.9 | 1902.5 |  122304.89 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    52.313 ms |  1.94 |  33 |  638.0 | 1046.3 |    13970.7 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   527.146 ms |  1.00 | 667 | 1266.6 |  527.1 |  266667.25 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   228.141 ms |  0.43 | 667 | 2926.7 |  228.1 |  268005.44 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   701.649 ms |  1.33 | 667 |  951.6 |  701.6 |  266824.38 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,881.004 ms |  3.57 | 667 |  355.0 | 1881.0 | 2442318.09 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,103.896 ms |  2.09 | 667 |  604.9 | 1103.9 |  266847.07 KB |        1.00 |
