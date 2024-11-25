```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-RAOLFZ : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-RAOLFZ  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.91 ms |  1.00 | 41 | 3228.2 |  258.2 |   1.08 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.40 ms |  1.43 | 41 | 2265.4 |  367.9 |   1.11 KB |        1.03 |
| Sep_TrimUnescape           | Cols  | 50000 |  18.79 ms |  1.46 | 41 | 2218.5 |  375.7 |   1.11 KB |        1.03 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.42 ms |  1.58 | 41 | 2041.2 |  408.3 |   1.11 KB |        1.03 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 144.51 ms | 11.19 | 41 |  288.4 | 2890.1 | 451.72 KB |      419.75 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 144.37 ms | 11.18 | 41 |  288.7 | 2887.5 |  446.2 KB |      414.61 |
