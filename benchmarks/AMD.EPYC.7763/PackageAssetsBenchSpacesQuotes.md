```

BenchmarkDotNet v0.15.1, Linux Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-BFPPER : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-BFPPER  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.70 ms |  1.00 | 41 | 3282.3 |  253.9 |   1.07 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.17 ms |  1.43 | 41 | 2293.5 |  363.4 |    1.1 KB |        1.02 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.20 ms |  1.51 | 41 | 2170.4 |  384.0 |   1.11 KB |        1.03 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.91 ms |  1.65 | 41 | 1992.9 |  418.2 |   1.87 KB |        1.74 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 144.27 ms | 11.36 | 41 |  288.9 | 2885.3 | 457.89 KB |      426.65 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 145.88 ms | 11.49 | 41 |  285.7 | 2917.6 |  446.2 KB |      415.75 |
