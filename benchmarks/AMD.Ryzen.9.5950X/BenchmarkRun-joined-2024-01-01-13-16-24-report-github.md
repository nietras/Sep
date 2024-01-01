```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-QRUVQP : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-QRUVQP  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350.0000 ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method          | Scope | Rows    | Mean         | Ratio | RatioSD | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------------- |------ |-------- |-------------:|------:|--------:|----:|-------:|-------:|-----------:|------------:|
| **Sep______**       | **Asset** | **50000**   |    **21.967 ms** |  **1.00** |    **0.00** |  **29** | **1328.4** |  **439.3** |   **13.48 MB** |        **1.00** |
| Sep_MT___       | Asset | 50000   |     7.213 ms |  0.33 |    0.01 |  29 | 4045.9 |  144.3 |   13.64 MB |        1.01 |
| Sylvan___       | Asset | 50000   |    29.561 ms |  1.35 |    0.00 |  29 |  987.2 |  591.2 |   13.63 MB |        1.01 |
| List            | Asset | 50000   |     3.592 ms |  0.16 |    0.02 |  29 | 8124.9 |   71.8 |   13.21 MB |        0.98 |
| ReadLine_       | Asset | 50000   |    38.109 ms |  1.68 |    0.36 |  29 |  765.7 |  762.2 |   99.74 MB |        7.40 |
| CsvHelper       | Asset | 50000   |    79.085 ms |  3.60 |    0.02 |  29 |  369.0 | 1581.7 |   13.64 MB |        1.01 |
| RecordParser_MT | Asset | 50000   |    14.831 ms |  0.68 |    0.01 |  29 | 1967.5 |  296.6 |   26.42 MB |        1.96 |
|                 |       |         |              |       |         |     |        |        |            |             |
| **Sep______**       | **Asset** | **1000000** |   **445.132 ms** |  **1.00** |    **0.00** | **583** | **1311.5** |  **445.1** |  **260.41 MB** |        **1.00** |
| Sep_MT___       | Asset | 1000000 |   134.569 ms |  0.30 |    0.01 | 583 | 4338.2 |  134.6 |  261.48 MB |        1.00 |
| Sylvan___       | Asset | 1000000 |   599.706 ms |  1.34 |    0.01 | 583 |  973.4 |  599.7 |  260.57 MB |        1.00 |
| List            | Asset | 1000000 |    73.058 ms |  0.17 |    0.04 | 583 | 7990.7 |   73.1 |  260.14 MB |        1.00 |
| ReadLine_       | Asset | 1000000 |   675.416 ms |  1.51 |    0.01 | 583 |  864.3 |  675.4 | 1991.04 MB |        7.65 |
| CsvHelper       | Asset | 1000000 | 1,580.350 ms |  3.55 |    0.03 | 583 |  369.4 | 1580.3 |  260.58 MB |        1.00 |
| RecordParser_MT | Asset | 1000000 |   249.491 ms |  0.56 |    0.01 | 583 | 2339.9 |  249.5 |  363.62 MB |        1.40 |
