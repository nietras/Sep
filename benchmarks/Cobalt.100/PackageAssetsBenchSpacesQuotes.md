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
| Sep_                       | Cols  | 50000 |  13.44 ms |  1.00 | 41 | 3107.1 |  268.9 |     960 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.30 ms |  1.36 | 41 | 2282.0 |  366.1 |     960 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.39 ms |  1.44 | 41 | 2154.4 |  387.8 |     963 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.98 ms |  1.56 | 41 | 1991.3 |  419.5 |     960 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 119.79 ms |  8.91 | 41 |  348.7 | 2395.9 |  462174 B |      481.43 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 115.93 ms |  8.63 | 41 |  360.3 | 2318.6 |  456368 B |      475.38 |
