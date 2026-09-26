```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 3.08GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.38 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.54 ms |  1.00 | 41 | 3323.4 |  250.8 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  17.79 ms |  1.42 | 41 | 2342.7 |  355.8 |   1.02 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.52 ms |  1.56 | 41 | 2135.0 |  390.4 |   1.02 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.06 ms |  1.68 | 41 | 1978.9 |  421.2 |   1.02 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 143.60 ms | 11.45 | 41 |  290.2 | 2872.1 | 451.27 KB |      444.32 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 144.56 ms | 11.53 | 41 |  288.3 | 2891.3 |  445.6 KB |      438.75 |
