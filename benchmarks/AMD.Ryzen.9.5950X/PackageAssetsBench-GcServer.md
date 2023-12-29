```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-DFUXPF : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-DFUXPF  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350.0000 ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean         | Ratio | RatioSD | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|--------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    22.138 ms |  1.00 |    0.00 |  29 | 1318.2 |  442.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     7.222 ms |  0.32 |    0.00 |  29 | 4040.6 |  144.4 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    30.112 ms |  1.36 |    0.02 |  29 |  969.1 |  602.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    39.837 ms |  1.86 |    0.41 |  29 |  732.5 |  796.7 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    79.581 ms |  3.59 |    0.02 |  29 |  366.7 | 1591.6 |   13.64 MB |        1.01 |
|           |       |         |              |       |         |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   450.151 ms |  1.00 |    0.00 | 583 | 1296.9 |  450.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   135.709 ms |  0.30 |    0.01 | 583 | 4301.7 |  135.7 |  261.36 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   598.959 ms |  1.33 |    0.01 | 583 |  974.7 |  599.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   654.982 ms |  1.46 |    0.02 | 583 |  891.3 |  655.0 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,592.052 ms |  3.53 |    0.03 | 583 |  366.7 | 1592.1 |  260.58 MB |        1.00 |
