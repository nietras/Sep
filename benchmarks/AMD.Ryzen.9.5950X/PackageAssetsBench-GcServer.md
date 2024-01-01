```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-CUMBMW : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-CUMBMW  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350.0000 ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method          | Scope | Rows    | Mean         | Ratio | RatioSD | MB  | MB/s    | ns/row | Allocated  | Alloc Ratio |
|---------------- |------ |-------- |-------------:|------:|--------:|----:|--------:|-------:|-----------:|------------:|
| Sep______       | Asset | 50000   |    22.345 ms |  1.00 |    0.00 |  29 |  1306.0 |  446.9 |   13.48 MB |        1.00 |
| Sep_MT___       | Asset | 50000   |     7.137 ms |  0.32 |    0.00 |  29 |  4088.9 |  142.7 |   13.65 MB |        1.01 |
| Sylvan___       | Asset | 50000   |    30.280 ms |  1.35 |    0.01 |  29 |   963.7 |  605.6 |   13.63 MB |        1.01 |
| List            | Asset | 50000   |     3.572 ms |  0.17 |    0.02 |  29 |  8169.3 |   71.4 |   13.21 MB |        0.98 |
| List_MT         | Asset | 50000   |     1.661 ms |  0.07 |    0.00 |  29 | 17564.7 |   33.2 |   12.98 MB |        0.96 |
| ReadLine_       | Asset | 50000   |    37.873 ms |  1.73 |    0.29 |  29 |   770.5 |  757.5 |   99.74 MB |        7.40 |
| CsvHelper       | Asset | 50000   |    79.705 ms |  3.57 |    0.04 |  29 |   366.1 | 1594.1 |   13.64 MB |        1.01 |
| RecordParser_MT | Asset | 50000   |    14.654 ms |  0.65 |    0.00 |  29 |  1991.3 |  293.1 |   26.41 MB |        1.96 |
|                 |       |         |              |       |         |     |         |        |            |             |
| Sep______       | Asset | 1000000 |   448.799 ms |  1.00 |    0.00 | 583 |  1300.8 |  448.8 |  260.41 MB |        1.00 |
| Sep_MT___       | Asset | 1000000 |   133.253 ms |  0.30 |    0.00 | 583 |  4381.0 |  133.3 |  261.38 MB |        1.00 |
| Sylvan___       | Asset | 1000000 |   602.699 ms |  1.34 |    0.00 | 583 |   968.6 |  602.7 |  260.57 MB |        1.00 |
| List            | Asset | 1000000 |    72.952 ms |  0.17 |    0.04 | 583 |  8002.3 |   73.0 |  260.14 MB |        1.00 |
| List_MT         | Asset | 1000000 |    37.615 ms |  0.08 |    0.00 | 583 | 15520.1 |   37.6 |  259.43 MB |        1.00 |
| ReadLine_       | Asset | 1000000 |   662.954 ms |  1.48 |    0.01 | 583 |   880.6 |  663.0 | 1991.05 MB |        7.65 |
| CsvHelper       | Asset | 1000000 | 1,592.513 ms |  3.54 |    0.02 | 583 |   366.6 | 1592.5 |  260.58 MB |        1.00 |
| RecordParser_MT | Asset | 1000000 |   253.276 ms |  0.56 |    0.01 | 583 |  2304.9 |  253.3 |  363.73 MB |        1.40 |
