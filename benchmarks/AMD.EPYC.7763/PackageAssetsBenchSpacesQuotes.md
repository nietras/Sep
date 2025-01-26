```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.1 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-GAKWOE : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-GAKWOE  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.81 ms |  1.00 | 41 | 3252.5 |  256.3 |   1.07 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.01 ms |  1.41 | 41 | 2314.0 |  360.2 |   1.11 KB |        1.03 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.33 ms |  1.51 | 41 | 2155.6 |  386.7 |    1.1 KB |        1.03 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.03 ms |  1.64 | 41 | 1981.6 |  420.6 |   1.87 KB |        1.74 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 144.52 ms | 11.28 | 41 |  288.4 | 2890.5 | 451.87 KB |      421.03 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 145.45 ms | 11.35 | 41 |  286.5 | 2909.0 | 446.21 KB |      415.76 |
