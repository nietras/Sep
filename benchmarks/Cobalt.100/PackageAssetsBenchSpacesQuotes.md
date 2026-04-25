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
| Sep_                       | Cols  | 50000 |  14.27 ms |  1.00 | 41 | 2928.0 |  285.3 |     962 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.48 ms |  1.30 | 41 | 2260.4 |  369.6 |     963 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.26 ms |  1.35 | 41 | 2168.9 |  385.2 |     963 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.00 ms |  1.47 | 41 | 1989.4 |  419.9 |     960 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 119.85 ms |  8.40 | 41 |  348.5 | 2396.9 |  462174 B |      480.43 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 115.73 ms |  8.11 | 41 |  360.9 | 2314.5 |  456374 B |      474.40 |
