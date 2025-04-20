```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.4 (23H420) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-HYWXRS : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-HYWXRS  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean     | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |---------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 | 12.21 ms |  1.00 | 41 | 3413.5 |  244.2 |   1.09 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 16.12 ms |  1.32 | 41 | 2584.8 |  322.5 |   1.19 KB |        1.09 |
| Sep_TrimUnescape           | Cols  | 50000 | 16.54 ms |  1.35 | 41 | 2519.4 |  330.8 |   1.23 KB |        1.13 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 20.24 ms |  1.66 | 41 | 2059.5 |  404.7 |   1.04 KB |        0.96 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 88.79 ms |  7.27 | 41 |  469.3 | 1775.8 |  451.6 KB |      415.49 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 87.00 ms |  7.13 | 41 |  479.0 | 1740.0 | 445.93 KB |      410.27 |
