```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.202
  [Host]     : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2
  Job-RZQSGR : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2

Job=Job-RZQSGR  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350.0000 ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    25.84 ms |  1.00 |  33 | 1291.8 |  516.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    10.97 ms |  0.42 |  33 | 3041.8 |  219.5 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    45.45 ms |  1.70 |  33 |  734.3 |  909.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    40.09 ms |  1.59 |  33 |  832.6 |  801.7 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    92.69 ms |  3.59 |  33 |  360.1 | 1853.8 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   546.80 ms |  1.00 | 667 | 1221.1 |  546.8 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   212.94 ms |  0.39 | 667 | 3135.7 |  212.9 |  261.47 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   960.19 ms |  1.75 | 667 |  695.4 |  960.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   678.16 ms |  1.24 | 667 |  984.6 |  678.2 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,900.80 ms |  3.47 | 667 |  351.3 | 1900.8 |  260.58 MB |        1.00 |
