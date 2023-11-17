```

BenchmarkDotNet v0.13.10, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-TUSNIO : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-TUSNIO  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=False  
Reader=String  

```
| Method       | Scope | Rows  | Mean       | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |------ |-----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000 |   2.409 ms |  1.00 | 29 | 12111.6 |   48.2 |       933 B |        1.00 |
| Sep_Unescape | Row   | 50000 |   2.499 ms |  1.04 | 29 | 11675.7 |   50.0 |       934 B |        1.00 |
| Sylvan___    | Row   | 50000 |   2.913 ms |  1.21 | 29 | 10016.5 |   58.3 |      7381 B |        7.91 |
| ReadLine_    | Row   | 50000 |  12.198 ms |  5.04 | 29 |  2392.3 |  244.0 |  90734838 B |   97,250.63 |
| CsvHelper    | Row   | 50000 |  42.460 ms | 17.63 | 29 |   687.3 |  849.2 |     21074 B |       22.59 |
|              |       |       |            |       |    |         |        |             |             |
| Sep______    | Cols  | 50000 |   3.480 ms |  1.00 | 29 |  8385.3 |   69.6 |       935 B |        1.00 |
| Sep_Unescape | Cols  | 50000 |   3.959 ms |  1.14 | 29 |  7370.9 |   79.2 |       936 B |        1.00 |
| Sylvan___    | Cols  | 50000 |   5.210 ms |  1.50 | 29 |  5600.5 |  104.2 |      7385 B |        7.90 |
| ReadLine_    | Cols  | 50000 |  14.275 ms |  3.60 | 29 |  2044.3 |  285.5 |  90734826 B |   97,042.59 |
| CsvHelper    | Cols  | 50000 |  69.058 ms | 19.86 | 29 |   422.6 | 1381.2 |    457060 B |      488.83 |
|              |       |       |            |       |    |         |        |             |             |
| Sep______    | Asset | 50000 |  35.052 ms |  1.00 | 29 |   832.5 |  701.0 |  14131121 B |        1.00 |
| Sep_Unescape | Asset | 50000 |  34.813 ms |  1.00 | 29 |   838.2 |  696.3 |  14130768 B |        1.00 |
| Sylvan___    | Asset | 50000 |  38.875 ms |  1.12 | 29 |   750.6 |  777.5 |  14297351 B |        1.01 |
| ReadLine_    | Asset | 50000 | 127.866 ms |  3.76 | 29 |   228.2 | 2557.3 | 104584604 B |        7.40 |
| CsvHelper    | Asset | 50000 |  86.415 ms |  2.47 | 29 |   337.7 | 1728.3 |  14306352 B |        1.01 |
