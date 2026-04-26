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
| Sep_                       | Cols  | 50000 |  13.21 ms |  1.00 | 41 | 3162.0 |  264.2 |     960 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  17.43 ms |  1.32 | 41 | 2396.3 |  348.6 |     960 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  18.75 ms |  1.42 | 41 | 2227.2 |  375.1 |     961 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.59 ms |  1.56 | 41 | 2029.1 |  411.7 |     960 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 119.36 ms |  9.04 | 41 |  349.9 | 2387.3 |  462174 B |      481.43 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 116.01 ms |  8.78 | 41 |  360.0 | 2320.3 |  456374 B |      475.39 |
