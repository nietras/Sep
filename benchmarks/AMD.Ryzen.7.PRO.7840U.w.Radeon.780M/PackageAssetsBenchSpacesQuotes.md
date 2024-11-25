```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-YBSRVP : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-YBSRVP  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  13.26 ms |  1.00 | 41 | 3149.0 |  265.3 |   1.22 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.75 ms |  1.41 | 41 | 2228.2 |  374.9 |   1.24 KB |        1.02 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.82 ms |  1.49 | 41 | 2107.2 |  396.5 |   1.25 KB |        1.02 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  22.54 ms |  1.70 | 41 | 1853.0 |  450.8 |   1.27 KB |        1.04 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 129.86 ms |  9.79 | 41 |  321.7 | 2597.2 | 454.53 KB |      372.95 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 127.98 ms |  9.65 | 41 |  326.4 | 2559.5 | 445.86 KB |      365.83 |
