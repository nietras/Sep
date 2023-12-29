```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.17763.3287/1809/October2018Update/Redstone5)
Intel Xeon Silver 4316 CPU 2.30GHz, 1 CPU, 40 logical and 20 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-OACVFI : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-OACVFI  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350.0000 ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean         | Ratio | RatioSD | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|--------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    41.669 ms |  1.00 |    0.00 |  29 |  700.3 |  833.4 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     9.802 ms |  0.24 |    0.00 |  29 | 2977.1 |  196.0 |   13.68 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    56.608 ms |  1.36 |    0.01 |  29 |  515.5 | 1132.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    47.549 ms |  1.14 |    0.01 |  29 |  613.7 |  951.0 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   153.359 ms |  3.68 |    0.02 |  29 |  190.3 | 3067.2 |   13.64 MB |        1.01 |
|           |       |         |              |       |         |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   822.268 ms |  1.00 |    0.00 | 583 |  710.0 |  822.3 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   161.654 ms |  0.20 |    0.00 | 583 | 3611.3 |  161.7 |  261.31 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,120.677 ms |  1.36 |    0.00 | 583 |  520.9 | 1120.7 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,081.121 ms |  1.31 |    0.00 | 583 |  540.0 | 1081.1 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 3,051.024 ms |  3.71 |    0.00 | 583 |  191.3 | 3051.0 |  260.58 MB |        1.00 |
