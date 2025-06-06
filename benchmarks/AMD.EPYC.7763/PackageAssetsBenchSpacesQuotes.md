```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-MPBGVI : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-MPBGVI  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.80 ms |  1.00 | 41 | 3257.0 |  255.9 |   1.07 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  17.87 ms |  1.40 | 41 | 2332.4 |  357.4 |    1.1 KB |        1.02 |
| Sep_TrimUnescape           | Cols  | 50000 |  18.95 ms |  1.48 | 41 | 2199.2 |  379.0 |    1.1 KB |        1.03 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.33 ms |  1.67 | 41 | 1953.6 |  426.6 |   1.11 KB |        1.04 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 145.44 ms | 11.37 | 41 |  286.5 | 2908.9 | 451.86 KB |      421.02 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 143.57 ms | 11.22 | 41 |  290.3 | 2871.4 |  446.2 KB |      415.75 |
