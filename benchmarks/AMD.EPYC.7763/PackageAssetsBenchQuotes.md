```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.59 GB Available
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
| Sep______    | Row   | 50000   |    10.30 ms |  1.00 |  33 | 3241.8 |  205.9 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    10.56 ms |  1.03 |  33 | 3161.4 |  211.2 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    11.13 ms |  1.08 |  33 | 2998.4 |  222.6 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    25.95 ms |  2.52 |  33 | 1286.3 |  518.9 |       8.47 KB |        8.34 |
| ReadLine_    | Row   | 50000   |    22.03 ms |  2.14 |  33 | 1514.9 |  440.6 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Row   | 50000   |    79.14 ms |  7.69 |  33 |  421.7 | 1582.9 |      19.95 KB |       19.64 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    11.35 ms |  1.00 |  33 | 2940.6 |  227.0 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.36 ms |  1.18 |  33 | 2498.8 |  267.2 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    29.85 ms |  2.63 |  33 | 1118.0 |  597.1 |       8.47 KB |        8.34 |
| ReadLine_    | Cols  | 50000   |    24.77 ms |  2.18 |  33 | 1347.4 |  495.4 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Cols  | 50000   |   119.39 ms | 10.52 |  33 |  279.6 | 2387.9 |      445.6 KB |      438.75 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    58.55 ms |  1.00 |  33 |  570.0 | 1171.0 |   13803.23 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    40.10 ms |  0.69 |  33 |  832.4 |  802.0 |   13892.74 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    71.11 ms |  1.22 |  33 |  469.4 | 1422.2 |   13962.03 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   184.24 ms |  3.15 |  33 |  181.2 | 3684.8 |  122304.99 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   165.07 ms |  2.82 |  33 |  202.2 | 3301.4 |   13969.95 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,105.43 ms |  1.00 | 667 |  604.0 | 1105.4 |  266668.01 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   702.26 ms |  0.64 | 667 |  950.8 |  702.3 |  272144.23 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,551.21 ms |  1.40 | 667 |  430.4 | 1551.2 |  266825.04 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 4,091.57 ms |  3.70 | 667 |  163.2 | 4091.6 | 2442320.05 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,662.25 ms |  2.41 | 667 |  250.8 | 2662.3 |  266839.08 KB |        1.00 |
