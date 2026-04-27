```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 2.56GHz), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  15.12 ms |  1.00 | 41 | 2756.7 |  302.3 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  20.35 ms |  1.35 | 41 | 2047.7 |  407.0 |   1.02 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  20.48 ms |  1.36 | 41 | 2035.1 |  409.6 |   1.02 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  22.77 ms |  1.51 | 41 | 1830.5 |  455.3 |   1.02 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 134.81 ms |  8.92 | 41 |  309.1 | 2696.1 | 451.34 KB |      444.40 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 133.12 ms |  8.81 | 41 |  313.1 | 2662.4 | 445.67 KB |      438.82 |
