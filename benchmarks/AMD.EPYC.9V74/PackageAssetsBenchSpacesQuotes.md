```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.34 GB Available
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
| Sep_                       | Cols  | 50000 |  12.45 ms |  1.00 | 41 | 3347.8 |  249.0 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  17.99 ms |  1.45 | 41 | 2316.8 |  359.8 |   1.02 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.68 ms |  1.58 | 41 | 2117.4 |  393.6 |   1.02 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  22.68 ms |  1.82 | 41 | 1837.3 |  453.6 |   1.02 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 128.79 ms | 10.35 | 41 |  323.6 | 2575.8 | 451.27 KB |      444.32 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 126.42 ms | 10.16 | 41 |  329.6 | 2528.5 |  445.6 KB |      438.75 |
