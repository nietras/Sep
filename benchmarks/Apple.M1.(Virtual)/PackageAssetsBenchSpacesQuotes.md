```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.2 (23H311) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD
  Job-ILBOFO : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD

Job=Job-ILBOFO  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean     | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |---------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 | 12.75 ms |  1.00 | 41 | 3269.7 |  254.9 |   1.09 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 16.65 ms |  1.31 | 41 | 2502.5 |  333.1 |   1.03 KB |        0.95 |
| Sep_TrimUnescape           | Cols  | 50000 | 16.27 ms |  1.28 | 41 | 2561.5 |  325.4 |   1.03 KB |        0.95 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 20.01 ms |  1.57 | 41 | 2082.3 |  400.3 |   1.37 KB |        1.26 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 88.95 ms |  6.98 | 41 |  468.5 | 1779.1 | 451.61 KB |      415.50 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 87.80 ms |  6.89 | 41 |  474.7 | 1755.9 | 445.86 KB |      410.21 |
