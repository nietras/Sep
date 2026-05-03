```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8246/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.87 ms |  1.00 | 41 | 3244.6 |  257.5 |     968 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  17.48 ms |  1.36 | 41 | 2390.1 |  349.5 |     970 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  18.18 ms |  1.41 | 41 | 2297.6 |  363.6 |     968 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.10 ms |  1.56 | 41 | 2078.6 |  401.9 |     968 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 119.89 ms |  9.32 | 41 |  348.4 | 2397.9 |  462174 B |      477.45 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 115.61 ms |  8.98 | 41 |  361.3 | 2312.1 |  456374 B |      471.46 |
