```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  Job-ZHUZEU : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2

Job=Job-ZHUZEU  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=True  
Reader=String  

```
| Method                     | Scope | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |-----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |   8.698 ms |  1.00 | 41 | 4802.1 |  174.0 |   1.03 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  11.816 ms |  1.36 | 41 | 3535.0 |  236.3 |   1.04 KB |        1.01 |
| Sep_TrimUnescape           | Cols  | 50000 |  12.110 ms |  1.39 | 41 | 3449.2 |  242.2 |   1.05 KB |        1.01 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  13.573 ms |  1.56 | 41 | 3077.5 |  271.5 |   1.05 KB |        1.02 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 115.212 ms | 13.25 | 41 |  362.5 | 2304.2 | 451.43 KB |      436.51 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 111.501 ms | 12.82 | 41 |  374.6 | 2230.0 | 445.76 KB |      431.03 |
