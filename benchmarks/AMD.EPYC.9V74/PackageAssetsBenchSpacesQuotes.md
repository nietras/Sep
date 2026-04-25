```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.86GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep_                       | Cols  | 50000 |  12.68 ms |  1.00 | 41 | 3285.4 |  253.7 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.27 ms |  1.44 | 41 | 2280.9 |  365.4 |   1.02 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.80 ms |  1.56 | 41 | 2104.7 |  396.0 |   1.02 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  22.55 ms |  1.78 | 41 | 1848.3 |  451.0 |   1.02 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 129.73 ms | 10.23 | 41 |  321.2 | 2594.6 | 451.34 KB |      444.40 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 129.52 ms | 10.21 | 41 |  321.8 | 2590.4 | 445.68 KB |      438.82 |
