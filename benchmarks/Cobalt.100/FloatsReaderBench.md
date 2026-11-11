```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26200.9725/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 11.96 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep______ | Row    | 25000 |  3.430 ms |  1.00 | 20 | 5923.4 |  137.2 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 | 23.159 ms |  6.75 | 20 |  877.4 |  926.4 |  16.56 KB |        4.34 |
| CsvHelper | Row    | 25000 |  8.072 ms |  2.35 | 20 | 2517.2 |  322.9 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Cols   | 25000 |  4.613 ms |  1.00 | 20 | 4405.2 |  184.5 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 | 26.076 ms |  5.65 | 20 |  779.2 | 1043.0 |  16.56 KB |        4.34 |
| CsvHelper | Cols   | 25000 |  9.305 ms |  2.02 | 20 | 2183.9 |  372.2 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Floats | 25000 | 32.199 ms |  1.00 | 20 |  631.1 | 1287.9 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  9.569 ms |  0.30 | 20 | 2123.6 |  382.7 |  51.18 KB |        8.75 |
| Sylvan___ | Floats | 25000 | 88.592 ms |  2.75 | 20 |  229.4 | 3543.7 |  18.24 KB |        3.12 |
| CsvHelper | Floats | 25000 | 38.656 ms |  1.20 | 20 |  525.7 | 1546.2 |   6.09 KB |        1.04 |
