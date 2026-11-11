```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26200.9725/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 12.13 GB Available
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
| Sep______ | Row    | 25000 |  3.371 ms |  1.00 | 20 | 6028.4 |  134.8 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 | 23.235 ms |  6.90 | 20 |  874.5 |  929.4 |  16.56 KB |        4.34 |
| CsvHelper | Row    | 25000 |  8.040 ms |  2.39 | 20 | 2527.2 |  321.6 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Cols   | 25000 |  4.564 ms |  1.00 | 20 | 4452.0 |  182.6 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 | 26.026 ms |  5.70 | 20 |  780.7 | 1041.0 |  16.56 KB |        4.34 |
| CsvHelper | Cols   | 25000 |  9.191 ms |  2.01 | 20 | 2210.9 |  367.6 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Floats | 25000 | 30.027 ms |  1.00 | 20 |  676.7 | 1201.1 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  9.392 ms |  0.31 | 20 | 2163.5 |  375.7 |  52.53 KB |        8.98 |
| Sylvan___ | Floats | 25000 | 87.052 ms |  2.90 | 20 |  233.4 | 3482.1 |  18.24 KB |        3.12 |
| CsvHelper | Floats | 25000 | 38.649 ms |  1.29 | 20 |  525.8 | 1546.0 |   6.09 KB |        1.04 |
