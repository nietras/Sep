```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.71GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep_                       | Cols  | 50000 |  13.18 ms |  1.00 | 41 | 3161.1 |  263.7 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.66 ms |  1.42 | 41 | 2233.7 |  373.1 |   1.03 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.30 ms |  1.46 | 41 | 2159.4 |  386.0 |   1.03 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.60 ms |  1.64 | 41 | 1929.7 |  431.9 |   1.03 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 142.21 ms | 10.79 | 41 |  293.0 | 2844.2 | 451.34 KB |      441.00 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 142.44 ms | 10.81 | 41 |  292.6 | 2848.8 | 445.67 KB |      435.47 |
