```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 9950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-RXSQJG : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-RXSQJG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  5.235 ms |  1.00 | 41 | 7978.5 |  104.7 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  7.953 ms |  1.52 | 41 | 5252.0 |  159.1 |   1.04 KB |        1.02 |
| Sep_TrimUnescape           | Cols  | 50000 |  8.544 ms |  1.63 | 41 | 4888.7 |  170.9 |   1.04 KB |        1.02 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  9.254 ms |  1.77 | 41 | 4513.7 |  185.1 |   1.05 KB |        1.02 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 62.406 ms | 11.92 | 41 |  669.3 | 1248.1 | 451.34 KB |      440.58 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 61.412 ms | 11.73 | 41 |  680.2 | 1228.2 | 445.72 KB |      435.10 |
