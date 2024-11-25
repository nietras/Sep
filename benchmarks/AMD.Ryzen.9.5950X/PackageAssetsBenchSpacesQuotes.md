```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2
  Job-LKXTKX : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-LKXTKX  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  8.599 ms |  1.00 | 41 | 4857.5 |  172.0 |   1.04 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 12.402 ms |  1.44 | 41 | 3368.0 |  248.0 |   1.05 KB |        1.01 |
| Sep_TrimUnescape           | Cols  | 50000 | 13.201 ms |  1.54 | 41 | 3164.1 |  264.0 |   1.06 KB |        1.02 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 14.568 ms |  1.69 | 41 | 2867.3 |  291.4 |   1.07 KB |        1.02 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 96.272 ms | 11.20 | 41 |  433.9 | 1925.4 | 451.52 KB |      432.51 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 95.183 ms | 11.07 | 41 |  438.8 | 1903.7 | 445.86 KB |      427.09 |
