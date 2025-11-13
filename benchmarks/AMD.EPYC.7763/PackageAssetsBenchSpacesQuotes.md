```

BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763 2.90GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.98 ms |  1.00 | 41 | 3211.7 |  259.5 |   1.01 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  19.39 ms |  1.49 | 41 | 2148.8 |  387.9 |   1.01 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.45 ms |  1.50 | 41 | 2142.7 |  389.0 |   1.01 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.53 ms |  1.66 | 41 | 1935.4 |  430.7 |   1.01 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 142.47 ms | 10.98 | 41 |  292.5 | 2849.5 | 451.34 KB |      447.84 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 141.73 ms | 10.92 | 41 |  294.1 | 2834.5 | 445.68 KB |      442.23 |
