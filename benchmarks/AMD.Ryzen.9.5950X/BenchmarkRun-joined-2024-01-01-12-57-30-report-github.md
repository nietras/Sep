```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-FZTYNX : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-FZTYNX  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=False  
Reader=String  

```
| Method          | Scope | Rows    | Mean        | Ratio | RatioSD | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------------- |------ |-------- |------------:|------:|--------:|----:|-------:|-------:|-----------:|------------:|
| **Sep______**       | **Asset** | **50000**   |    **38.07 ms** |  **1.00** |    **0.00** |  **29** |  **766.5** |  **761.5** |   **13.48 MB** |        **1.00** |
| Sep_MT___       | Asset | 50000   |    25.37 ms |  0.67 |    0.03 |  29 | 1150.2 |  507.4 |   13.66 MB |        1.01 |
| Sylvan___       | Asset | 50000   |    38.87 ms |  1.02 |    0.04 |  29 |  750.8 |  777.3 |   13.64 MB |        1.01 |
| List            | Asset | 50000   |    10.25 ms |  0.27 |    0.01 |  29 | 2846.9 |  205.0 |   13.21 MB |        0.98 |
| ReadLine_       | Asset | 50000   |   106.56 ms |  2.80 |    0.05 |  29 |  273.8 | 2131.2 |   99.74 MB |        7.40 |
| CsvHelper       | Asset | 50000   |    85.64 ms |  2.29 |    0.06 |  29 |  340.7 | 1712.8 |   13.64 MB |        1.01 |
| RecordParser_MT | Asset | 50000   |    54.87 ms |  1.46 |    0.04 |  29 |  531.8 | 1097.4 |   26.47 MB |        1.96 |
|                 |       |         |             |       |         |     |        |        |            |             |
| **Sep______**       | **Asset** | **1000000** |   **674.44 ms** |  **1.00** |    **0.00** | **583** |  **865.6** |  **674.4** |  **260.42 MB** |        **1.00** |
| Sep_MT___       | Asset | 1000000 |   309.52 ms |  0.46 |    0.01 | 583 | 1886.1 |  309.5 |  261.39 MB |        1.00 |
| Sylvan___       | Asset | 1000000 |   815.40 ms |  1.21 |    0.01 | 583 |  716.0 |  815.4 |  260.57 MB |        1.00 |
| List            | Asset | 1000000 |   226.37 ms |  0.33 |    0.00 | 583 | 2578.8 |  226.4 |  260.14 MB |        1.00 |
| ReadLine_       | Asset | 1000000 | 1,953.93 ms |  2.90 |    0.03 | 583 |  298.8 | 1953.9 | 1991.05 MB |        7.65 |
| CsvHelper       | Asset | 1000000 | 1,761.35 ms |  2.61 |    0.01 | 583 |  331.4 | 1761.4 |  260.58 MB |        1.00 |
| RecordParser_MT | Asset | 1000000 |   769.24 ms |  1.14 |    0.02 | 583 |  758.9 |  769.2 |  363.51 MB |        1.40 |
