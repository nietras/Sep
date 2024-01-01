```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-FZTYNX : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-FZTYNX  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=True  
Reader=String  

```
| Method          | Scope | Rows    | Mean        | Ratio | RatioSD | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------------- |------ |-------- |------------:|------:|--------:|----:|-------:|-------:|-----------:|------------:|
| **Sep______**       | **Asset** | **50000**   |    **34.94 ms** |  **1.00** |    **0.00** |  **33** |  **955.3** |  **698.8** |   **13.48 MB** |        **1.00** |
| Sep_MT___       | Asset | 50000   |    27.05 ms |  0.77 |    0.04 |  33 | 1234.1 |  540.9 |   13.66 MB |        1.01 |
| Sylvan___       | Asset | 50000   |    52.49 ms |  1.53 |    0.03 |  33 |  635.9 | 1049.8 |   13.64 MB |        1.01 |
| List            | Asset | 50000   |    10.28 ms |  0.30 |    0.01 |  33 | 3246.9 |  205.6 |   13.21 MB |        0.98 |
| ReadLine_       | Asset | 50000   |   116.79 ms |  3.34 |    0.11 |  33 |  285.8 | 2335.8 |  119.44 MB |        8.86 |
| CsvHelper       | Asset | 50000   |    99.45 ms |  2.89 |    0.06 |  33 |  335.6 | 1988.9 |   13.64 MB |        1.01 |
| RecordParser_MT | Asset | 50000   |    76.64 ms |  2.19 |    0.10 |  33 |  435.5 | 1532.7 |   27.87 MB |        2.07 |
|                 |       |         |             |       |         |     |        |        |            |             |
| **Sep______**       | **Asset** | **1000000** |   **779.60 ms** |  **1.00** |    **0.00** | **667** |  **856.5** |  **779.6** |  **260.42 MB** |        **1.00** |
| Sep_MT___       | Asset | 1000000 |   385.60 ms |  0.50 |    0.01 | 667 | 1731.6 |  385.6 |  261.49 MB |        1.00 |
| Sylvan___       | Asset | 1000000 | 1,114.26 ms |  1.43 |    0.02 | 667 |  599.2 | 1114.3 |  260.57 MB |        1.00 |
| List            | Asset | 1000000 |   223.05 ms |  0.28 |    0.01 | 667 | 2993.5 |  223.1 |  260.14 MB |        1.00 |
| ReadLine_       | Asset | 1000000 | 2,669.27 ms |  3.42 |    0.04 | 667 |  250.1 | 2669.3 | 2385.07 MB |        9.16 |
| CsvHelper       | Asset | 1000000 | 2,042.56 ms |  2.63 |    0.03 | 667 |  326.9 | 2042.6 |  260.59 MB |        1.00 |
| RecordParser_MT | Asset | 1000000 | 1,352.30 ms |  1.74 |    0.02 | 667 |  493.8 | 1352.3 |  371.93 MB |        1.43 |
