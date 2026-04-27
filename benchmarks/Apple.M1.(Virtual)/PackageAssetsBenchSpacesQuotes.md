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
| Sep_                       | Cols  | 50000 |  11.99 ms |  1.01 | 41 | 3475.8 |  239.8 |     960 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  15.90 ms |  1.34 | 41 | 2621.4 |  318.0 |     960 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  15.67 ms |  1.32 | 41 | 2659.8 |  313.4 |     960 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  18.55 ms |  1.56 | 41 | 2247.0 |  370.9 |     960 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 100.12 ms |  8.43 | 41 |  416.2 | 2002.4 |  462096 B |      481.35 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 105.95 ms |  8.93 | 41 |  393.3 | 2119.1 |  456296 B |      475.31 |
