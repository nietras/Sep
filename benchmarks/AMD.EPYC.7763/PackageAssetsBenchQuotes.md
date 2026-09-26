```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.61 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    11.96 ms |  1.00 |  33 | 2791.6 |  239.1 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    10.26 ms |  0.86 |  33 | 3253.5 |  205.2 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.79 ms |  0.90 |  33 | 3093.6 |  215.8 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    26.96 ms |  2.26 |  33 | 1238.2 |  539.1 |       8.47 KB |        8.34 |
| ReadLine_    | Row   | 50000   |    23.31 ms |  1.95 |  33 | 1431.7 |  466.3 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Row   | 50000   |    88.77 ms |  7.43 |  33 |  376.0 | 1775.4 |      19.95 KB |       19.64 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    14.52 ms |  1.00 |  33 | 2298.6 |  290.4 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.55 ms |  0.93 |  33 | 2463.9 |  270.9 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    28.33 ms |  1.95 |  33 | 1178.3 |  566.6 |       8.47 KB |        8.34 |
| ReadLine_    | Cols  | 50000   |    21.34 ms |  1.47 |  33 | 1564.1 |  426.8 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Cols  | 50000   |   121.58 ms |  8.38 |  33 |  274.5 | 2431.6 |      445.6 KB |      438.75 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    67.71 ms |  1.00 |  33 |  492.9 | 1354.2 |   13802.17 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    40.25 ms |  0.60 |  33 |  829.3 |  805.0 |   13920.19 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    70.00 ms |  1.04 |  33 |  476.8 | 1400.0 |   13962.42 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   179.40 ms |  2.66 |  33 |  186.1 | 3588.0 |  122304.81 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   126.43 ms |  1.88 |  33 |  264.0 | 2528.6 |   13969.95 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,082.89 ms |  1.00 | 667 |  616.6 | 1082.9 |  266667.95 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   712.56 ms |  0.66 | 667 |  937.1 |  712.6 |  279609.45 KB |        1.05 |
| Sylvan___    | Asset | 1000000 | 1,542.74 ms |  1.42 | 667 |  432.8 | 1542.7 |  266825.01 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,631.69 ms |  3.35 | 667 |  183.9 | 3631.7 | 2442320.22 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,636.51 ms |  2.43 | 667 |  253.3 | 2636.5 |  266844.92 KB |        1.00 |
