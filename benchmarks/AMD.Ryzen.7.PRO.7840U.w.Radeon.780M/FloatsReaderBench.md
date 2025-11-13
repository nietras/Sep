```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26100.6899/24H2/2024Update/HudsonValley)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics 3.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.090 ms |  1.00 | 20 | 6576.8 |  123.6 |     1.24 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.662 ms |  1.19 | 20 | 5548.9 |  146.5 |     10.7 KB |        8.61 |
| ReadLine_ | Row    | 25000 |  14.759 ms |  4.78 | 20 | 1376.7 |  590.4 | 73489.62 KB |   59,161.45 |
| CsvHelper | Row    | 25000 |  40.346 ms | 13.06 | 20 |  503.6 | 1613.9 |    19.95 KB |       16.06 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.244 ms |  1.00 | 20 | 4787.9 |  169.8 |     1.24 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.501 ms |  1.53 | 20 | 3125.5 |  260.1 |     10.7 KB |        8.61 |
| ReadLine_ | Cols   | 25000 |  15.460 ms |  3.64 | 20 | 1314.3 |  618.4 | 73489.62 KB |   59,161.45 |
| CsvHelper | Cols   | 25000 |  42.710 ms | 10.06 | 20 |  475.8 | 1708.4 | 21340.16 KB |   17,179.50 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  30.251 ms |  1.00 | 20 |  671.7 | 1210.1 |     7.89 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   6.924 ms |  0.23 | 20 | 2934.7 |  277.0 |   111.53 KB |       14.13 |
| Sylvan___ | Floats | 25000 |  78.705 ms |  2.60 | 20 |  258.2 | 3148.2 |     18.7 KB |        2.37 |
| ReadLine_ | Floats | 25000 | 102.918 ms |  3.40 | 20 |  197.4 | 4116.7 | 73492.94 KB |    9,313.96 |
| CsvHelper | Floats | 25000 | 156.097 ms |  5.16 | 20 |  130.2 | 6243.9 | 22061.22 KB |    2,795.88 |
