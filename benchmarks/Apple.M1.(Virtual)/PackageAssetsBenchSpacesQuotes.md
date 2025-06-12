```

BenchmarkDotNet v0.15.1, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-AKIMDM : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-AKIMDM  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  9.479 ms |  1.00 | 41 | 4396.3 |  189.6 |   1.17 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 13.018 ms |  1.37 | 41 | 3201.2 |  260.4 |   1.36 KB |        1.16 |
| Sep_TrimUnescape           | Cols  | 50000 | 40.026 ms |  4.22 | 41 | 1041.2 |  800.5 |   1.36 KB |        1.16 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 39.957 ms |  4.22 | 41 | 1043.0 |  799.1 |   1.36 KB |        1.16 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 90.642 ms |  9.56 | 41 |  459.8 | 1812.8 | 451.53 KB |      384.98 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 87.946 ms |  9.28 | 41 |  473.9 | 1758.9 | 445.93 KB |      380.21 |
