```

BenchmarkDotNet v0.16.0-preview.1, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
Memory: 7 GB Total, 0.09 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean     | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |---------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 | 10.47 ms |  1.00 | 41 | 3978.8 |  209.5 |     960 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 14.69 ms |  1.40 | 41 | 2836.8 |  293.8 |     960 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 | 17.93 ms |  1.71 | 41 | 2324.8 |  358.5 |     960 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 19.41 ms |  1.86 | 41 | 2146.5 |  388.3 |     960 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 97.53 ms |  9.32 | 41 |  427.3 | 1950.6 |  462096 B |      481.35 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 90.24 ms |  8.62 | 41 |  461.8 | 1804.9 |  456296 B |      475.31 |
