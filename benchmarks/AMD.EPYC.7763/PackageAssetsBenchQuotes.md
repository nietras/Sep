```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 3.13GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______    | Row   | 50000   |    11.49 ms |  1.00 |  33 | 2895.8 |  229.9 |      1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    11.41 ms |  0.99 |  33 | 2917.5 |  228.2 |      1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.94 ms |  0.95 |  33 | 3041.2 |  218.9 |      1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    24.91 ms |  2.17 |  33 | 1336.0 |  498.3 |      8.47 KB |        8.34 |
| ReadLine_    | Row   | 50000   |    27.87 ms |  2.42 |  33 | 1194.3 |  557.3 | 108778.73 KB |  107,105.21 |
| CsvHelper    | Row   | 50000   |    77.78 ms |  6.77 |  33 |  427.9 | 1555.6 |     20.02 KB |       19.71 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    12.93 ms |  1.00 |  33 | 2574.2 |  258.6 |      1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.21 ms |  1.02 |  33 | 2519.9 |  264.2 |      1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    28.34 ms |  2.19 |  33 | 1174.4 |  566.8 |      8.47 KB |        8.34 |
| ReadLine_    | Cols  | 50000   |    27.60 ms |  2.14 |  33 | 1205.7 |  552.1 | 108778.73 KB |  107,105.21 |
| CsvHelper    | Cols  | 50000   |   106.04 ms |  8.20 |  33 |  313.9 | 2120.7 |    445.67 KB |      438.82 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    46.75 ms |  1.00 |  33 |  711.9 |  935.1 |  13802.21 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    31.93 ms |  0.68 |  33 | 1042.2 |  638.7 |  13861.33 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    69.51 ms |  1.49 |  33 |  478.8 | 1390.1 |  13961.95 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   165.96 ms |  3.55 |  33 |  200.5 | 3319.3 | 122305.14 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   127.47 ms |  2.73 |  33 |  261.1 | 2549.4 |  13970.36 KB |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   980.61 ms |  1.00 | 665 |  679.0 |  980.6 | 266667.11 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   591.90 ms |  0.60 | 665 | 1124.9 |  591.9 | 273166.01 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,399.93 ms |  1.43 | 665 |  475.6 | 1399.9 | 266824.24 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,512.95 ms |  3.58 | 665 |  189.5 | 3513.0 | 2442318.2 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,578.88 ms |  2.63 | 665 |  258.2 | 2578.9 | 266836.13 KB |        1.00 |
