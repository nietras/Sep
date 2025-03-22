```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 9950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.103
  [Host]     : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-MIRFZN : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-MIRFZN  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  5.357 ms |  1.00 | 41 | 7797.8 |  107.1 |   1.19 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  7.988 ms |  1.49 | 41 | 5229.4 |  159.8 |    1.2 KB |        1.01 |
| Sep_TrimUnescape           | Cols  | 50000 |  8.569 ms |  1.60 | 41 | 4874.6 |  171.4 |    1.2 KB |        1.01 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  9.295 ms |  1.74 | 41 | 4493.8 |  185.9 |    1.2 KB |        1.01 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 63.102 ms | 11.78 | 41 |  661.9 | 1262.0 | 451.39 KB |      380.74 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 60.368 ms | 11.27 | 41 |  691.9 | 1207.4 | 445.72 KB |      375.96 |
