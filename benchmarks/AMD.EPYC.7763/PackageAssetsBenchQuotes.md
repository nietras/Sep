```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.82GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    11.35 ms |  1.00 |  33 | 2931.6 |  227.1 |      1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    11.23 ms |  0.99 |  33 | 2963.8 |  224.6 |      1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.90 ms |  0.96 |  33 | 3052.6 |  218.1 |      1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    24.73 ms |  2.18 |  33 | 1345.8 |  494.6 |      8.47 KB |        8.34 |
| ReadLine_    | Row   | 50000   |    27.29 ms |  2.40 |  33 | 1219.7 |  545.7 | 108778.73 KB |  107,105.21 |
| CsvHelper    | Row   | 50000   |    77.68 ms |  6.84 |  33 |  428.4 | 1553.7 |     19.95 KB |       19.64 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    12.69 ms |  1.00 |  33 | 2622.8 |  253.8 |      1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.20 ms |  1.04 |  33 | 2521.1 |  264.0 |      1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    28.09 ms |  2.21 |  33 | 1184.8 |  561.8 |      8.47 KB |        8.34 |
| ReadLine_    | Cols  | 50000   |    27.92 ms |  2.20 |  33 | 1191.9 |  558.4 | 108778.73 KB |  107,105.21 |
| CsvHelper    | Cols  | 50000   |   106.39 ms |  8.38 |  33 |  312.8 | 2127.8 |     445.6 KB |      438.75 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    44.99 ms |  1.00 |  33 |  739.8 |  899.7 |  13804.11 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    32.23 ms |  0.72 |  33 | 1032.5 |  644.7 |  13865.38 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    67.53 ms |  1.50 |  33 |  492.9 | 1350.6 |  13962.44 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   148.70 ms |  3.31 |  33 |  223.8 | 2974.0 | 122304.33 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   125.96 ms |  2.80 |  33 |  264.2 | 2519.3 |  13970.22 KB |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   959.88 ms |  1.00 | 665 |  693.6 |  959.9 | 266667.38 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   577.27 ms |  0.60 | 665 | 1153.4 |  577.3 | 271839.64 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,378.06 ms |  1.44 | 665 |  483.1 | 1378.1 |  266824.3 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,118.91 ms |  3.25 | 665 |  213.5 | 3118.9 | 2442317.3 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,567.91 ms |  2.68 | 665 |  259.3 | 2567.9 | 266847.02 KB |        1.00 |
