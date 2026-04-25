```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
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
| Sep_                       | Cols  | 50000 |  8.953 ms |  1.00 | 41 | 4655.0 |  179.1 |     960 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 12.411 ms |  1.39 | 41 | 3357.8 |  248.2 |     960 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 | 13.070 ms |  1.46 | 41 | 3188.5 |  261.4 |     960 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 14.613 ms |  1.63 | 41 | 2851.8 |  292.3 |     960 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 88.212 ms |  9.85 | 41 |  472.4 | 1764.2 |  462096 B |      481.35 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 86.002 ms |  9.61 | 41 |  484.6 | 1720.0 |  459464 B |      478.61 |
