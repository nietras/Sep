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
| Sep_                       | Cols  | 50000 |  13.32 ms |  1.00 | 41 | 3135.6 |  266.4 |     960 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  17.66 ms |  1.33 | 41 | 2365.2 |  353.2 |     960 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  18.50 ms |  1.39 | 41 | 2257.8 |  370.0 |     960 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.43 ms |  1.53 | 41 | 2044.7 |  408.6 |     960 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 119.10 ms |  8.94 | 41 |  350.7 | 2382.0 |  462174 B |      481.43 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 116.16 ms |  8.72 | 41 |  359.6 | 2323.3 |  456374 B |      475.39 |
