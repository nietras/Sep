```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-PZPUMS : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-PZPUMS  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350.0000 ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method          | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------------- |------ |-------- |-----------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______       | Asset | 50000   |  22.233 ms |  1.00 |  29 | 1312.5 |  444.7 |  13.48 MB |        1.00 |
| Sep_MT___       | Asset | 50000   |   7.204 ms |  0.32 |  29 | 4050.5 |  144.1 |  13.65 MB |        1.01 |
| RecordParser_MT | Asset | 50000   |  14.881 ms |  0.68 |  29 | 1961.0 |  297.6 |  26.42 MB |        1.96 |
|                 |       |         |            |       |     |        |        |           |             |
| Sep______       | Asset | 1000000 | 448.874 ms |  1.00 | 583 | 1300.5 |  448.9 | 260.41 MB |        1.00 |
| Sep_MT___       | Asset | 1000000 | 136.539 ms |  0.31 | 583 | 4275.6 |  136.5 | 261.71 MB |        1.00 |
| RecordParser_MT | Asset | 1000000 | 253.169 ms |  0.57 | 583 | 2305.9 |  253.2 | 363.01 MB |        1.39 |
