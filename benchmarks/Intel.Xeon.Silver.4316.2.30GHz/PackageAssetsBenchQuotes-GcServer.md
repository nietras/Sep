```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.17763.3287/1809/October2018Update/Redstone5)
Intel Xeon Silver 4316 CPU 2.30GHz, 1 CPU, 40 logical and 20 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-OACVFI : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-OACVFI  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350.0000 ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | RatioSD | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|--------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    52.58 ms |  1.00 |    0.00 |  33 |  634.7 | 1051.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    17.79 ms |  0.34 |    0.00 |  33 | 1875.9 |  355.9 |   13.67 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    86.54 ms |  1.65 |    0.01 |  33 |  385.7 | 1730.8 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    55.36 ms |  1.05 |    0.01 |  33 |  602.9 | 1107.1 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   176.28 ms |  3.35 |    0.02 |  33 |  189.3 | 3525.6 |   13.64 MB |        1.01 |
|           |       |         |             |       |         |     |        |        |            |             |
| Sep______ | Asset | 1000000 | 1,043.85 ms |  1.00 |    0.00 | 667 |  639.7 | 1043.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   336.86 ms |  0.32 |    0.00 | 667 | 1982.1 |  336.9 |  261.35 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,697.87 ms |  1.63 |    0.00 | 667 |  393.3 | 1697.9 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,177.60 ms |  1.13 |    0.00 | 667 |  567.0 | 1177.6 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 3,478.19 ms |  3.33 |    0.01 | 667 |  192.0 | 3478.2 |  260.58 MB |        1.00 |
