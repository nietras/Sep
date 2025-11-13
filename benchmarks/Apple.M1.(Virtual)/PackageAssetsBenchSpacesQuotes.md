```

BenchmarkDotNet v0.15.6, macOS Sequoia 15.7.1 (24G231) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.36 ms |  1.01 | 41 | 3373.0 |  247.1 |     952 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  19.17 ms |  1.56 | 41 | 2174.4 |  383.3 |     952 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  20.35 ms |  1.66 | 41 | 2047.8 |  407.0 |     952 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  17.07 ms |  1.39 | 41 | 2441.6 |  341.4 |     952 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 110.49 ms |  8.99 | 41 |  377.2 | 2209.7 |  462096 B |      485.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 130.51 ms | 10.62 | 41 |  319.3 | 2610.1 |  456368 B |      479.38 |
