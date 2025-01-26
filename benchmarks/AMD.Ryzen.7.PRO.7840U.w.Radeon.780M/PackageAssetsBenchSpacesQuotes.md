```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4602/23H2/2023Update/SunValley3)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-VCJIGY : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-VCJIGY  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  13.52 ms |  1.00 | 41 | 3089.2 |  270.4 |   1.22 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.53 ms |  1.37 | 41 | 2253.9 |  370.6 |   1.91 KB |        1.57 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.87 ms |  1.47 | 41 | 2102.1 |  397.4 |   1.25 KB |        1.02 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  22.41 ms |  1.66 | 41 | 1863.6 |  448.3 |   1.26 KB |        1.03 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 129.55 ms |  9.58 | 41 |  322.4 | 2591.0 | 451.52 KB |      369.89 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 127.82 ms |  9.45 | 41 |  326.8 | 2556.4 | 445.86 KB |      365.25 |
