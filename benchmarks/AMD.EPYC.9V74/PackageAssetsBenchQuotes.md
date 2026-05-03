```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    11.13 ms |  1.00 |  33 | 2990.3 |  222.6 |       1.03 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    10.98 ms |  0.99 |  33 | 3029.9 |  219.7 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.58 ms |  0.95 |  33 | 3147.0 |  211.5 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    24.21 ms |  2.17 |  33 | 1374.9 |  484.1 |       8.47 KB |        8.26 |
| ReadLine_    | Row   | 50000   |    26.61 ms |  2.39 |  33 | 1250.9 |  532.1 |  108778.73 KB |  106,085.16 |
| CsvHelper    | Row   | 50000   |    72.63 ms |  6.53 |  33 |  458.2 | 1452.6 |      20.02 KB |       19.53 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    12.63 ms |  1.00 |  33 | 2634.7 |  252.6 |       1.03 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.08 ms |  1.04 |  33 | 2543.9 |  261.7 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    28.55 ms |  2.26 |  33 | 1165.8 |  571.0 |       8.47 KB |        8.26 |
| ReadLine_    | Cols  | 50000   |    28.32 ms |  2.24 |  33 | 1175.2 |  566.4 |  108778.73 KB |  106,085.16 |
| CsvHelper    | Cols  | 50000   |   101.60 ms |  8.04 |  33 |  327.6 | 2032.0 |     445.68 KB |      434.64 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    47.22 ms |  1.00 |  33 |  704.8 |  944.4 |   13802.23 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    33.02 ms |  0.70 |  33 | 1008.0 |  660.4 |   13861.87 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    69.86 ms |  1.48 |  33 |  476.4 | 1397.2 |   13962.44 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   162.93 ms |  3.45 |  33 |  204.3 | 3258.6 |  122305.24 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   125.12 ms |  2.65 |  33 |  266.0 | 2502.3 |   13970.78 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,002.94 ms |  1.00 | 665 |  663.8 | 1002.9 |  266667.23 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   625.15 ms |  0.62 | 665 | 1065.0 |  625.2 |  271923.98 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,426.34 ms |  1.42 | 665 |  466.8 | 1426.3 |  266824.19 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,570.52 ms |  3.56 | 665 |  186.5 | 3570.5 | 2442318.23 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,532.66 ms |  2.53 | 665 |  262.9 | 2532.7 |  266845.17 KB |        1.00 |
