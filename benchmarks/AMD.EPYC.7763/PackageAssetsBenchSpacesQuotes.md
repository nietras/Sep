```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 3.13GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep_                       | Cols  | 50000 |  13.21 ms |  1.00 | 41 | 3154.7 |  264.2 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.45 ms |  1.40 | 41 | 2259.4 |  368.9 |   1.02 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.40 ms |  1.47 | 41 | 2148.5 |  387.9 |   1.02 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.73 ms |  1.64 | 41 | 1918.2 |  434.5 |   1.02 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 141.79 ms | 10.73 | 41 |  293.9 | 2835.7 | 451.35 KB |      444.40 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 141.62 ms | 10.72 | 41 |  294.3 | 2832.3 | 445.67 KB |      438.82 |
