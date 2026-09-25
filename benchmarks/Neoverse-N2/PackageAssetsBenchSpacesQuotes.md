```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 11.44 GB Available
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
| Sep_                       | Cols  | 50000 |  13.33 ms |  1.00 | 41 | 3125.5 |  266.7 |     960 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  17.63 ms |  1.32 | 41 | 2363.4 |  352.7 |     960 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  18.40 ms |  1.38 | 41 | 2264.8 |  368.0 |     960 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.15 ms |  1.51 | 41 | 2068.3 |  403.0 |     960 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 118.83 ms |  8.91 | 41 |  350.7 | 2376.6 |  462096 B |      481.35 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 115.18 ms |  8.64 | 41 |  361.8 | 2303.6 |  456296 B |      475.31 |
