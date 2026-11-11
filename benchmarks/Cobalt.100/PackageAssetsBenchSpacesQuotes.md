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
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean     | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |---------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 | 11.00 ms |  1.00 | 41 | 3797.4 |  220.0 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 15.48 ms |  1.41 | 41 | 2698.5 |  309.6 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 | 18.06 ms |  1.64 | 41 | 2313.0 |  361.2 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 19.74 ms |  1.79 | 41 | 2115.6 |  394.9 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 74.99 ms |  6.82 | 41 |  557.0 | 1499.9 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 82.00 ms |  7.45 | 41 |  509.4 | 1639.9 |    1264 B |        4.39 |
