```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.82GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep_                       | Cols  | 50000 |  12.98 ms |  1.00 | 41 | 3210.4 |  259.6 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.19 ms |  1.40 | 41 | 2290.8 |  363.8 |   1.02 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.16 ms |  1.48 | 41 | 2175.3 |  383.2 |   1.02 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.34 ms |  1.64 | 41 | 1952.7 |  426.8 |   1.02 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 142.00 ms | 10.94 | 41 |  293.5 | 2840.0 | 451.34 KB |      443.54 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 144.41 ms | 11.12 | 41 |  288.6 | 2888.1 | 445.67 KB |      437.97 |
