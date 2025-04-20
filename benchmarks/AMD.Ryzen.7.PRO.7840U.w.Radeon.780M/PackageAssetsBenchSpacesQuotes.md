```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-XBPEID : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-XBPEID  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  13.81 ms |  1.00 | 41 | 3023.7 |  276.3 |   1.07 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  19.30 ms |  1.40 | 41 | 2163.7 |  386.1 |   1.09 KB |        1.02 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.84 ms |  1.44 | 41 | 2105.6 |  396.7 |   1.09 KB |        1.02 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  22.54 ms |  1.63 | 41 | 1853.3 |  450.8 |    1.1 KB |        1.03 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 129.64 ms |  9.39 | 41 |  322.2 | 2592.7 | 454.54 KB |      426.63 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 127.70 ms |  9.25 | 41 |  327.1 | 2554.0 | 445.86 KB |      418.48 |
