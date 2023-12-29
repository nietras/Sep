```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-PZPUMS : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-PZPUMS  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350.0000 ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method          | Scope | Rows    | Mean      | Ratio | RatioSD | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------------- |------ |-------- |----------:|------:|--------:|----:|-------:|-------:|----------:|------------:|
| Sep______       | Asset | 50000   |  27.15 ms |  1.00 |    0.00 |  33 | 1229.1 |  543.1 |  13.48 MB |        1.00 |
| Sep_MT___       | Asset | 50000   |  12.97 ms |  0.48 |    0.00 |  33 | 2573.1 |  259.4 |  13.64 MB |        1.01 |
| RecordParser_MT | Asset | 50000   |  42.31 ms |  1.56 |    0.03 |  33 |  788.9 |  846.2 |  27.82 MB |        2.06 |
|                 |       |         |           |       |         |     |        |        |           |             |
| Sep______       | Asset | 1000000 | 556.24 ms |  1.00 |    0.00 | 667 | 1200.4 |  556.2 | 260.41 MB |        1.00 |
| Sep_MT___       | Asset | 1000000 | 232.72 ms |  0.42 |    0.01 | 667 | 2869.1 |  232.7 | 261.56 MB |        1.00 |
| RecordParser_MT | Asset | 1000000 | 783.29 ms |  1.41 |    0.01 | 667 |  852.4 |  783.3 | 370.61 MB |        1.42 |
