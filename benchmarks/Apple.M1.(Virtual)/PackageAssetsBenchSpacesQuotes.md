```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.1 (23H222) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  Job-PJJVEM : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD

Job=Job-PJJVEM  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean     | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |---------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 | 12.25 ms |  1.00 | 41 | 3400.6 |  245.1 |   1.01 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 15.81 ms |  1.29 | 41 | 2636.8 |  316.1 |   1.19 KB |        1.18 |
| Sep_TrimUnescape           | Cols  | 50000 | 16.32 ms |  1.33 | 41 | 2552.9 |  326.5 |   1.03 KB |        1.02 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 21.39 ms |  1.75 | 41 | 1948.6 |  427.7 |   1.37 KB |        1.36 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 89.28 ms |  7.29 | 41 |  466.8 | 1785.7 |  451.6 KB |      446.80 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 87.48 ms |  7.14 | 41 |  476.4 | 1749.5 | 445.93 KB |      441.19 |
