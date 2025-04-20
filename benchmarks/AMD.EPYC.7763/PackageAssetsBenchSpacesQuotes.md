```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-DRDGJI : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-DRDGJI  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.76 ms |  1.00 | 41 | 3267.2 |  255.1 |   1.07 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.28 ms |  1.43 | 41 | 2280.0 |  365.6 |    1.1 KB |        1.03 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.16 ms |  1.50 | 41 | 2174.6 |  383.3 |   1.77 KB |        1.65 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.42 ms |  1.60 | 41 | 2040.7 |  408.4 |   1.11 KB |        1.03 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 145.29 ms | 11.39 | 41 |  286.8 | 2905.7 | 451.87 KB |      421.03 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 143.57 ms | 11.26 | 41 |  290.3 | 2871.3 |  446.2 KB |      415.75 |
