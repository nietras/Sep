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
| Sep_                       | Cols  | 50000 |  8.974 ms |  1.00 | 41 | 4643.9 |  179.5 |     960 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 12.476 ms |  1.39 | 41 | 3340.3 |  249.5 |    1419 B |        1.48 |
| Sep_TrimUnescape           | Cols  | 50000 | 13.042 ms |  1.45 | 41 | 3195.4 |  260.8 |     960 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 14.246 ms |  1.59 | 41 | 2925.4 |  284.9 |     960 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 87.350 ms |  9.73 | 41 |  477.1 | 1747.0 |  462096 B |      481.35 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 86.014 ms |  9.58 | 41 |  484.5 | 1720.3 |  456368 B |      475.38 |
