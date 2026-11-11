```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26100.33674/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.55 GB Available
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
| Sep______ | Row    | 25000 |  3.104 ms |  1.00 | 20 | 6547.1 |  124.1 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  3.653 ms |  1.18 | 20 | 5562.4 |  146.1 |  16.94 KB |        4.44 |
| CsvHelper | Row    | 25000 |  3.783 ms |  1.22 | 20 | 5371.3 |  151.3 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Cols   | 25000 |  4.201 ms |  1.00 | 20 | 4837.1 |  168.0 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  6.359 ms |  1.51 | 20 | 3195.5 |  254.4 |  16.94 KB |        4.44 |
| CsvHelper | Cols   | 25000 |  5.132 ms |  1.22 | 20 | 3959.3 |  205.3 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Floats | 25000 | 29.811 ms |  1.00 | 20 |  681.6 | 1192.4 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 | 14.824 ms |  0.50 | 20 | 1370.8 |  592.9 |  57.37 KB |        9.80 |
| Sylvan___ | Floats | 25000 | 72.556 ms |  2.43 | 20 |  280.1 | 2902.2 |  18.62 KB |        3.18 |
| CsvHelper | Floats | 25000 | 34.847 ms |  1.17 | 20 |  583.1 | 1393.9 |   6.09 KB |        1.04 |
