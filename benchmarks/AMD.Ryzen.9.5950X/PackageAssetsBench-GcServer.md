```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.202
  [Host]     : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2
  Job-RZQSGR : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2

Job=Job-RZQSGR  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350.0000 ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    21.051 ms |  1.00 |  29 | 1386.2 |  421.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     5.993 ms |  0.29 |  29 | 4869.4 |  119.9 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    29.301 ms |  1.39 |  29 |  995.9 |  586.0 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    33.868 ms |  1.60 |  29 |  861.6 |  677.4 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    76.599 ms |  3.64 |  29 |  381.0 | 1532.0 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   425.355 ms |  1.00 | 583 | 1372.5 |  425.4 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   109.917 ms |  0.26 | 583 | 5311.1 |  109.9 |  261.49 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   588.226 ms |  1.38 | 583 |  992.4 |  588.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   581.137 ms |  1.37 | 583 | 1004.6 |  581.1 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,535.431 ms |  3.60 | 583 |  380.2 | 1535.4 |  260.58 MB |        1.00 |
