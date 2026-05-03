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
| Sep_                       | Cols  | 50000 |  9.034 ms |  1.00 | 41 | 4613.0 |  180.7 |     968 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 12.730 ms |  1.41 | 41 | 3273.7 |  254.6 |     968 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 | 13.231 ms |  1.46 | 41 | 3149.8 |  264.6 |     968 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 14.671 ms |  1.62 | 41 | 2840.7 |  293.4 |     970 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 87.315 ms |  9.67 | 41 |  477.3 | 1746.3 |  462168 B |      477.45 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 88.011 ms |  9.74 | 41 |  473.5 | 1760.2 |  456296 B |      471.38 |
