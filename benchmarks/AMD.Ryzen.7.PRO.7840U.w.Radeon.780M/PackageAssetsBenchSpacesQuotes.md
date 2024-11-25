```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-IHPSBG : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-FNCVNM : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Runtime  | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |--------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | .NET 8.0 | Cols  | 50000 |  13.83 ms |  1.00 | 41 | 3020.3 |  276.6 |   1.22 KB |        1.00 |
| Sep_Trim                   | .NET 8.0 | Cols  | 50000 |  18.77 ms |  1.36 | 41 | 2225.3 |  375.4 |   1.22 KB |        1.00 |
| Sep_TrimUnescape           | .NET 8.0 | Cols  | 50000 |  18.72 ms |  1.35 | 41 | 2231.6 |  374.4 |   1.22 KB |        1.00 |
| Sep_TrimUnescapeTrim       | .NET 8.0 | Cols  | 50000 |  21.74 ms |  1.57 | 41 | 1921.6 |  434.7 |   1.23 KB |        1.01 |
| CsvHelper_TrimUnescape     | .NET 8.0 | Cols  | 50000 | 155.86 ms | 11.27 | 41 |  268.0 | 3117.2 | 451.52 KB |      370.48 |
| CsvHelper_TrimUnescapeTrim | .NET 8.0 | Cols  | 50000 | 152.57 ms | 11.03 | 41 |  273.8 | 3051.4 | 445.86 KB |      365.83 |
| Sep_                       | .NET 9.0 | Cols  | 50000 |  13.19 ms |  0.95 | 41 | 3165.9 |  263.9 |   1.22 KB |        1.00 |
| Sep_Trim                   | .NET 9.0 | Cols  | 50000 |  18.81 ms |  1.36 | 41 | 2221.0 |  376.1 |   1.24 KB |        1.02 |
| Sep_TrimUnescape           | .NET 9.0 | Cols  | 50000 |  19.84 ms |  1.43 | 41 | 2105.6 |  396.7 |   1.25 KB |        1.02 |
| Sep_TrimUnescapeTrim       | .NET 9.0 | Cols  | 50000 |  22.59 ms |  1.63 | 41 | 1848.8 |  451.8 |   1.26 KB |        1.03 |
| CsvHelper_TrimUnescape     | .NET 9.0 | Cols  | 50000 | 129.95 ms |  9.40 | 41 |  321.4 | 2598.9 | 451.52 KB |      370.47 |
| CsvHelper_TrimUnescapeTrim | .NET 9.0 | Cols  | 50000 | 128.20 ms |  9.27 | 41 |  325.8 | 2564.0 | 448.88 KB |      368.31 |
