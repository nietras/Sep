```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-SOXNEC : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-SOXNEC  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350.0000 ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method          | Scope | Rows    | Mean         | Ratio | RatioSD | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------------- |------ |-------- |-------------:|------:|--------:|----:|-------:|-------:|-----------:|------------:|
| **Sep______**       | **Asset** | **50000**   |    **22.007 ms** |  **1.00** |    **0.00** |  **29** | **1326.0** |  **440.1** |   **13.48 MB** |        **1.00** |
| Sep_MT___       | Asset | 50000   |     7.125 ms |  0.32 |    0.00 |  29 | 4095.6 |  142.5 |   13.65 MB |        1.01 |
| Sylvan___       | Asset | 50000   |    29.523 ms |  1.34 |    0.00 |  29 |  988.4 |  590.5 |   13.63 MB |        1.01 |
| List            | Asset | 50000   |     3.650 ms |  0.17 |    0.02 |  29 | 7995.9 |   73.0 |   13.21 MB |        0.98 |
| ReadLine_       | Asset | 50000   |    37.918 ms |  1.70 |    0.35 |  29 |  769.6 |  758.4 |   99.74 MB |        7.40 |
| CsvHelper       | Asset | 50000   |    79.135 ms |  3.59 |    0.01 |  29 |  368.8 | 1582.7 |   13.65 MB |        1.01 |
| RecordParser_MT | Asset | 50000   |    14.673 ms |  0.67 |    0.00 |  29 | 1988.8 |  293.5 |    26.4 MB |        1.96 |
|                 |       |         |              |       |         |     |        |        |            |             |
| **Sep______**       | **Asset** | **1000000** |   **449.837 ms** |  **1.00** |    **0.00** | **583** | **1297.8** |  **449.8** |  **260.41 MB** |        **1.00** |
| Sep_MT___       | Asset | 1000000 |   130.665 ms |  0.29 |    0.00 | 583 | 4467.8 |  130.7 |  261.48 MB |        1.00 |
| Sylvan___       | Asset | 1000000 |   600.387 ms |  1.33 |    0.01 | 583 |  972.3 |  600.4 |  260.57 MB |        1.00 |
| List            | Asset | 1000000 |    73.820 ms |  0.17 |    0.03 | 583 | 7908.3 |   73.8 |  260.14 MB |        1.00 |
| ReadLine_       | Asset | 1000000 |   662.021 ms |  1.47 |    0.02 | 583 |  881.8 |  662.0 | 1991.04 MB |        7.65 |
| CsvHelper       | Asset | 1000000 | 1,598.979 ms |  3.55 |    0.02 | 583 |  365.1 | 1599.0 |  260.58 MB |        1.00 |
| RecordParser_MT | Asset | 1000000 |   249.758 ms |  0.55 |    0.00 | 583 | 2337.4 |  249.8 |  362.68 MB |        1.39 |
