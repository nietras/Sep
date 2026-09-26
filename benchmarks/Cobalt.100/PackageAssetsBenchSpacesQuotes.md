```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 12.01 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  13.30 ms |  1.00 | 41 | 3140.6 |  266.0 |     960 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  17.91 ms |  1.35 | 41 | 2332.1 |  358.2 |     960 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  18.87 ms |  1.42 | 41 | 2213.1 |  377.5 |     960 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.32 ms |  1.53 | 41 | 2055.9 |  406.3 |     960 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 118.60 ms |  8.92 | 41 |  352.2 | 2372.0 |  462096 B |      481.35 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 118.59 ms |  8.92 | 41 |  352.2 | 2371.8 |  456296 B |      475.31 |
