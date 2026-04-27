```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 3.24GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______    | Row   | 50000   |    11.42 ms |  1.00 |  33 | 2913.9 |  228.4 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    11.72 ms |  1.03 |  33 | 2840.1 |  234.4 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.82 ms |  0.95 |  33 | 3076.9 |  216.3 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    24.56 ms |  2.15 |  33 | 1355.4 |  491.1 |       8.47 KB |        8.34 |
| ReadLine_    | Row   | 50000   |    26.64 ms |  2.33 |  33 | 1249.5 |  532.7 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Row   | 50000   |    77.46 ms |  6.78 |  33 |  429.7 | 1549.2 |      19.95 KB |       19.64 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    12.66 ms |  1.00 |  33 | 2629.7 |  253.1 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.32 ms |  1.05 |  33 | 2498.0 |  266.5 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    28.17 ms |  2.23 |  33 | 1181.4 |  563.4 |       8.47 KB |        8.34 |
| ReadLine_    | Cols  | 50000   |    27.55 ms |  2.18 |  33 | 1208.2 |  550.9 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Cols  | 50000   |   106.05 ms |  8.38 |  33 |  313.8 | 2121.1 |     445.68 KB |      438.82 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    45.66 ms |  1.00 |  33 |  728.9 |  913.2 |   13802.21 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    31.85 ms |  0.70 |  33 | 1044.8 |  637.1 |   13865.49 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    68.62 ms |  1.50 |  33 |  485.0 | 1372.4 |   13961.94 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   153.43 ms |  3.36 |  33 |  216.9 | 3068.5 |  122305.14 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   127.77 ms |  2.80 |  33 |  260.5 | 2555.4 |   13971.34 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   968.61 ms |  1.00 | 665 |  687.4 |  968.6 |  266679.21 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   566.21 ms |  0.58 | 665 | 1175.9 |  566.2 |  271900.91 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,412.90 ms |  1.46 | 665 |  471.2 | 1412.9 |  266824.41 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,224.56 ms |  3.33 | 665 |  206.5 | 3224.6 | 2442318.43 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,561.55 ms |  2.64 | 665 |  259.9 | 2561.6 |   266835.7 KB |        1.00 |
