```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep_                       | Cols  | 50000 |  12.48 ms |  1.00 | 41 | 3339.8 |  249.6 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  17.97 ms |  1.44 | 41 | 2319.1 |  359.4 |   1.02 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.94 ms |  1.60 | 41 | 2089.8 |  398.8 |   1.02 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  22.56 ms |  1.81 | 41 | 1847.6 |  451.1 |   1.02 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 129.22 ms | 10.36 | 41 |  322.5 | 2584.4 | 451.34 KB |      441.00 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 128.93 ms | 10.33 | 41 |  323.2 | 2578.7 | 445.68 KB |      435.47 |
