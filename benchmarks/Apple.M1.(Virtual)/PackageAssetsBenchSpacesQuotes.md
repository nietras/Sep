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
| Sep_                       | Cols  | 50000 |  9.068 ms |  1.00 | 41 | 4595.6 |  181.4 |     960 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 12.424 ms |  1.37 | 41 | 3354.5 |  248.5 |     960 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 | 13.117 ms |  1.45 | 41 | 3177.1 |  262.3 |     960 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 14.325 ms |  1.58 | 41 | 2909.1 |  286.5 |     960 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 87.190 ms |  9.61 | 41 |  478.0 | 1743.8 |  462096 B |      481.35 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 86.163 ms |  9.50 | 41 |  483.7 | 1723.3 |  459458 B |      478.60 |
