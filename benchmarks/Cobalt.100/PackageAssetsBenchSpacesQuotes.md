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
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean     | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |---------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 | 11.01 ms |  1.00 | 41 | 3793.6 |  220.2 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 15.49 ms |  1.41 | 41 | 2696.9 |  309.8 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 | 18.17 ms |  1.65 | 41 | 2299.3 |  363.3 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 19.88 ms |  1.81 | 41 | 2101.4 |  397.5 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 75.05 ms |  6.82 | 41 |  556.6 | 1501.0 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 80.06 ms |  7.27 | 41 |  521.7 | 1601.2 |    1264 B |        4.39 |
