```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-SOXNEC : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-SOXNEC  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350.0000 ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method          | Scope | Rows    | Mean         | Ratio | RatioSD | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------------- |------ |-------- |-------------:|------:|--------:|----:|-------:|-------:|-----------:|------------:|
| **Sep______**       | **Asset** | **50000**   |    **26.935 ms** |  **1.00** |    **0.00** |  **33** | **1239.2** |  **538.7** |   **13.48 MB** |        **1.00** |
| Sep_MT___       | Asset | 50000   |    12.791 ms |  0.47 |    0.00 |  33 | 2609.5 |  255.8 |   13.64 MB |        1.01 |
| Sylvan___       | Asset | 50000   |    45.367 ms |  1.68 |    0.01 |  33 |  735.7 |  907.3 |   13.63 MB |        1.01 |
| List            | Asset | 50000   |     3.420 ms |  0.13 |    0.01 |  33 | 9758.1 |   68.4 |   13.21 MB |        0.98 |
| ReadLine_       | Asset | 50000   |    48.159 ms |  1.73 |    0.47 |  33 |  693.1 |  963.2 |  119.44 MB |        8.86 |
| CsvHelper       | Asset | 50000   |    96.074 ms |  3.67 |    0.08 |  33 |  347.4 | 1921.5 |   13.64 MB |        1.01 |
| RecordParser_MT | Asset | 50000   |    40.941 ms |  1.52 |    0.02 |  33 |  815.3 |  818.8 |    27.8 MB |        2.06 |
|                 |       |         |              |       |         |     |        |        |            |             |
| **Sep______**       | **Asset** | **1000000** |   **550.890 ms** |  **1.00** |    **0.00** | **667** | **1212.1** |  **550.9** |  **260.41 MB** |        **1.00** |
| Sep_MT___       | Asset | 1000000 |   239.518 ms |  0.43 |    0.01 | 667 | 2787.7 |  239.5 |   261.5 MB |        1.00 |
| Sylvan___       | Asset | 1000000 |   908.761 ms |  1.65 |    0.02 | 667 |  734.7 |  908.8 |  260.57 MB |        1.00 |
| List            | Asset | 1000000 |    83.254 ms |  0.14 |    0.04 | 667 | 8020.1 |   83.3 |  260.14 MB |        1.00 |
| ReadLine_       | Asset | 1000000 |   773.300 ms |  1.40 |    0.00 | 667 |  863.5 |  773.3 | 2385.07 MB |        9.16 |
| CsvHelper       | Asset | 1000000 | 1,913.927 ms |  3.47 |    0.02 | 667 |  348.9 | 1913.9 |  260.58 MB |        1.00 |
| RecordParser_MT | Asset | 1000000 |   819.428 ms |  1.51 |    0.01 | 667 |  814.8 |  819.4 |  371.13 MB |        1.43 |
