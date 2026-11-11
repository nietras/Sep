```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26100.33674/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.57 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep______ | Row    | 25000 |  2.995 ms |  1.00 | 20 | 6785.6 |  119.8 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  3.723 ms |  1.24 | 20 | 5457.7 |  148.9 |  16.94 KB |        4.44 |
| CsvHelper | Row    | 25000 |  3.796 ms |  1.27 | 20 | 5353.2 |  151.8 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Cols   | 25000 |  4.216 ms |  1.00 | 20 | 4819.5 |  168.6 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  6.327 ms |  1.50 | 20 | 3211.5 |  253.1 |  16.94 KB |        4.44 |
| CsvHelper | Cols   | 25000 |  5.068 ms |  1.20 | 20 | 4009.3 |  202.7 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Floats | 25000 | 29.990 ms |  1.00 | 20 |  677.5 | 1199.6 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 | 14.919 ms |  0.50 | 20 | 1362.0 |  596.8 |  57.13 KB |        9.76 |
| Sylvan___ | Floats | 25000 | 73.283 ms |  2.44 | 20 |  277.3 | 2931.3 |  18.62 KB |        3.18 |
| CsvHelper | Floats | 25000 | 34.922 ms |  1.16 | 20 |  581.9 | 1396.9 |   6.09 KB |        1.04 |
