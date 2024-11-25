```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.1 (23H222) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  Job-HKRCZO : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD

Job=Job-HKRCZO  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean     | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |---------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 | 12.70 ms |  1.00 | 41 | 3282.0 |  254.0 |   1.01 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 15.72 ms |  1.24 | 41 | 2650.5 |  314.5 |   1.19 KB |        1.17 |
| Sep_TrimUnescape           | Cols  | 50000 | 16.28 ms |  1.28 | 41 | 2559.8 |  325.6 |   1.03 KB |        1.02 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 21.21 ms |  1.67 | 41 | 1964.6 |  424.3 |   1.37 KB |        1.35 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 88.96 ms |  7.01 | 41 |  468.5 | 1779.2 |  451.6 KB |      445.94 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 86.97 ms |  6.85 | 41 |  479.2 | 1739.4 | 445.93 KB |      440.34 |
