```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 11.36 GB Available
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
| Sep_                       | Cols  | 50000 |  12.81 ms |  1.00 | 41 | 3252.2 |  256.3 |     960 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  17.77 ms |  1.39 | 41 | 2344.7 |  355.5 |     960 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  18.45 ms |  1.44 | 41 | 2258.2 |  369.1 |     960 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.35 ms |  1.59 | 41 | 2047.9 |  407.0 |     960 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 117.74 ms |  9.19 | 41 |  354.0 | 2354.8 |  462096 B |      481.35 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 117.08 ms |  9.14 | 41 |  355.9 | 2341.7 |  456296 B |      475.31 |
