```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 3.24GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.92 ms |  1.00 | 41 | 3224.4 |  258.5 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.18 ms |  1.41 | 41 | 2291.9 |  363.7 |   1.02 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.40 ms |  1.50 | 41 | 2148.0 |  388.0 |   1.02 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.31 ms |  1.65 | 41 | 1956.0 |  426.1 |   1.02 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 142.26 ms | 11.01 | 41 |  292.9 | 2845.2 | 451.34 KB |      444.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 141.52 ms | 10.95 | 41 |  294.5 | 2830.3 | 445.67 KB |      438.82 |
