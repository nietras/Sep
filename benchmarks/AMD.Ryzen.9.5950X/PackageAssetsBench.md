```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-WVXDVE : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-WVXDVE  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=False  
Reader=String  

```
| Method          | Scope | Rows    | Mean       | Ratio | RatioSD | MB  | MB/s    | ns/row | Allocated    | Alloc Ratio |
|---------------- |------ |-------- |-----------:|------:|--------:|----:|--------:|-------:|-------------:|------------:|
| Sep______       | Row   | 50000   |   2.327 ms |  1.00 |    0.00 |  29 | 12539.9 |   46.5 |      1.01 KB |        1.00 |
| Sep_Unescape    | Row   | 50000   |   2.362 ms |  1.02 |    0.00 |  29 | 12354.5 |   47.2 |      1.01 KB |        1.00 |
|                 |       |         |            |       |         |     |         |        |              |             |
| Sep______       | Cols  | 50000   |   3.216 ms |  1.00 |    0.00 |  29 |  9074.6 |   64.3 |      1.02 KB |        1.00 |
| Sep_Unescape    | Cols  | 50000   |   3.799 ms |  1.18 |    0.01 |  29 |  7680.8 |   76.0 |      1.02 KB |        1.00 |
|                 |       |         |            |       |         |     |         |        |              |             |
| Sep______       | Asset | 50000   |  35.924 ms |  1.00 |    0.00 |  29 |   812.3 |  718.5 |  13802.36 KB |        1.00 |
| Sep_MT___       | Asset | 50000   |  23.097 ms |  0.64 |    0.04 |  29 |  1263.4 |  461.9 |  13985.08 KB |        1.01 |
| RecordParser_MT | Asset | 50000   |  55.572 ms |  1.58 |    0.06 |  29 |   525.1 | 1111.4 |  27119.56 KB |        1.96 |
|                 |       |         |            |       |         |     |         |        |              |             |
| Sep______       | Asset | 1000000 | 670.815 ms |  1.00 |    0.00 | 583 |   870.3 |  670.8 | 266667.55 KB |        1.00 |
| Sep_MT___       | Asset | 1000000 | 303.552 ms |  0.45 |    0.03 | 583 |  1923.2 |  303.6 |  267474.8 KB |        1.00 |
| RecordParser_MT | Asset | 1000000 | 773.879 ms |  1.16 |    0.02 | 583 |   754.4 |  773.9 | 371652.31 KB |        1.39 |
