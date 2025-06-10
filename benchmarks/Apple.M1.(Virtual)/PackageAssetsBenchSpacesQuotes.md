```

BenchmarkDotNet v0.15.1, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-DUYDYG : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-DUYDYG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  9.440 ms |  1.00 | 41 | 4414.7 |  188.8 |    1000 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 13.026 ms |  1.38 | 41 | 3199.4 |  260.5 |    1388 B |        1.39 |
| Sep_TrimUnescape           | Cols  | 50000 | 40.196 ms |  4.26 | 41 | 1036.8 |  803.9 |    1388 B |        1.39 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 39.680 ms |  4.20 | 41 | 1050.3 |  793.6 |    1388 B |        1.39 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 89.899 ms |  9.52 | 41 |  463.6 | 1798.0 |  462436 B |      462.44 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 87.563 ms |  9.28 | 41 |  475.9 | 1751.3 |  456636 B |      456.64 |
