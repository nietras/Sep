```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2
  Job-WRHRFC : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2

Job=Job-WRHRFC  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  9.467 ms |  1.00 | 41 | 4412.2 |  189.3 |   1.05 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 12.972 ms |  1.37 | 41 | 3219.9 |  259.4 |   1.06 KB |        1.01 |
| Sep_TrimUnescape           | Cols  | 50000 | 13.630 ms |  1.44 | 41 | 3064.5 |  272.6 |   1.06 KB |        1.02 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 15.502 ms |  1.64 | 41 | 2694.4 |  310.0 |   1.07 KB |        1.03 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 98.444 ms | 10.40 | 41 |  424.3 | 1968.9 | 451.52 KB |      431.70 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 97.110 ms | 10.26 | 41 |  430.1 | 1942.2 | 445.86 KB |      426.29 |
