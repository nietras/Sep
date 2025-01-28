```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.1 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-XDFYGT : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-XDFYGT  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  13.32 ms |  1.00 | 41 | 3128.8 |  266.4 |   1.08 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  26.13 ms |  1.96 | 41 | 1595.1 |  522.5 |   1.11 KB |        1.02 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.37 ms |  1.45 | 41 | 2151.4 |  387.4 |   1.11 KB |        1.03 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.30 ms |  1.60 | 41 | 1956.4 |  426.0 |   1.11 KB |        1.03 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 145.66 ms | 10.94 | 41 |  286.1 | 2913.2 | 451.86 KB |      418.74 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 143.79 ms | 10.80 | 41 |  289.8 | 2875.8 |  446.2 KB |      413.49 |
