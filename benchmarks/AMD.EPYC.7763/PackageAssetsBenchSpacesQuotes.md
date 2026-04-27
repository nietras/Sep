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
| Sep_                       | Cols  | 50000 |  12.81 ms |  1.00 | 41 | 3252.7 |  256.2 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.09 ms |  1.41 | 41 | 2304.0 |  361.8 |   1.02 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.06 ms |  1.49 | 41 | 2186.6 |  381.2 |   1.02 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.46 ms |  1.67 | 41 | 1942.4 |  429.1 |   1.02 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 141.78 ms | 11.07 | 41 |  293.9 | 2835.7 | 451.34 KB |      444.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 140.58 ms | 10.97 | 41 |  296.4 | 2811.6 | 445.67 KB |      438.82 |
