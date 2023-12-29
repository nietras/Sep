```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-DFUXPF : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-DFUXPF  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350.0000 ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | RatioSD | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|--------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    26.83 ms |  1.00 |    0.00 |  33 | 1243.9 |  536.6 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    12.95 ms |  0.48 |    0.01 |  33 | 2576.6 |  259.1 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    44.44 ms |  1.66 |    0.01 |  33 |  751.1 |  888.8 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    46.86 ms |  1.71 |    0.48 |  33 |  712.3 |  937.2 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    93.47 ms |  3.48 |    0.02 |  33 |  357.1 | 1869.5 |   13.64 MB |        1.01 |
|           |       |         |             |       |         |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   552.21 ms |  1.00 |    0.00 | 667 | 1209.2 |  552.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   230.71 ms |  0.42 |    0.00 | 667 | 2894.2 |  230.7 |   261.6 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   928.39 ms |  1.68 |    0.01 | 667 |  719.2 |  928.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   750.83 ms |  1.36 |    0.02 | 667 |  889.3 |  750.8 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,852.26 ms |  3.35 |    0.01 | 667 |  360.5 | 1852.3 |  260.58 MB |        1.00 |
