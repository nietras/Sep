```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-NMHWMW : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-NMHWMW  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.88 ms |  1.00 | 41 | 3234.9 |  257.7 |   1.08 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.48 ms |  1.43 | 41 | 2255.6 |  369.5 |   1.11 KB |        1.03 |
| Sep_TrimUnescape           | Cols  | 50000 |  18.90 ms |  1.47 | 41 | 2204.6 |  378.1 |    1.1 KB |        1.02 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  25.51 ms |  1.98 | 41 | 1633.5 |  510.3 |   1.14 KB |        1.06 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 143.16 ms | 11.11 | 41 |  291.1 | 2863.2 | 451.86 KB |      419.88 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 144.43 ms | 11.21 | 41 |  288.5 | 2888.6 |  446.2 KB |      414.61 |
