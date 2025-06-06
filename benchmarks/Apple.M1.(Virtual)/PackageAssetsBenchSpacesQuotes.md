```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-SMYKWG : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-SMYKWG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean     | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |---------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 | 10.39 ms |  1.01 | 41 | 4009.6 |  207.9 |    1010 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 12.92 ms |  1.25 | 41 | 3224.6 |  258.5 |    1017 B |        1.01 |
| Sep_TrimUnescape           | Cols  | 50000 | 13.48 ms |  1.31 | 41 | 3091.4 |  269.6 |    1022 B |        1.01 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 17.19 ms |  1.67 | 41 | 2424.3 |  343.8 |    1388 B |        1.37 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 89.98 ms |  8.74 | 41 |  463.1 | 1799.6 |  462436 B |      457.86 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 88.17 ms |  8.56 | 41 |  472.7 | 1763.4 |  456636 B |      452.11 |
