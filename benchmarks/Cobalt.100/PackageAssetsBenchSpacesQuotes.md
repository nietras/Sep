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
| Sep_                       | Cols  | 50000 |  14.00 ms |  1.00 | 41 | 2982.7 |  280.1 |     961 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.29 ms |  1.31 | 41 | 2283.4 |  365.9 |     963 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  18.93 ms |  1.35 | 41 | 2206.8 |  378.6 |     961 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.80 ms |  1.49 | 41 | 2007.9 |  416.1 |     960 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 118.77 ms |  8.48 | 41 |  351.7 | 2375.4 |  462174 B |      480.93 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 116.77 ms |  8.34 | 41 |  357.7 | 2335.4 |  456374 B |      474.89 |
