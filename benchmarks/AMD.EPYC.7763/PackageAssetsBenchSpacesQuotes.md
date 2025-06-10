```

BenchmarkDotNet v0.15.1, Linux Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-GLYBTL : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-GLYBTL  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  13.02 ms |  1.00 | 41 | 3201.3 |  260.4 |   1.08 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.39 ms |  1.41 | 41 | 2266.5 |  367.7 |    1.1 KB |        1.02 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.23 ms |  1.48 | 41 | 2167.2 |  384.6 |   1.11 KB |        1.03 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.15 ms |  1.62 | 41 | 1970.4 |  423.0 |   1.12 KB |        1.04 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 144.90 ms | 11.13 | 41 |  287.6 | 2898.0 | 451.86 KB |      419.88 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 144.94 ms | 11.13 | 41 |  287.5 | 2898.7 |  446.2 KB |      414.61 |
