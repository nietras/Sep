```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.2 (23H311) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD
  Job-KNSVKR : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD

Job=Job-KNSVKR  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean     | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |---------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 | 13.51 ms |  1.00 | 41 | 3085.8 |  270.1 |    1.1 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 16.78 ms |  1.24 | 41 | 2484.2 |  335.5 |   1.03 KB |        0.94 |
| Sep_TrimUnescape           | Cols  | 50000 | 17.90 ms |  1.33 | 41 | 2327.7 |  358.1 |   1.37 KB |        1.25 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 21.65 ms |  1.60 | 41 | 1924.5 |  433.1 |   1.37 KB |        1.25 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 95.06 ms |  7.04 | 41 |  438.4 | 1901.1 |  451.6 KB |      410.32 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 93.41 ms |  6.92 | 41 |  446.1 | 1868.2 | 445.93 KB |      405.18 |
