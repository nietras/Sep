```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-SVHPFG : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-SVHPFG  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=True  
Reader=String  

```
| Method           | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|----------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______        | Row   | 50000   |     6.888 ms |  1.00 |  33 | 4845.6 |  137.8 |        981 B |        1.00 |
| Sep_Unescape     | Row   | 50000   |     6.685 ms |  0.97 |  33 | 4992.7 |  133.7 |        981 B |        1.00 |
| Sylvan___        | Row   | 50000   |    17.476 ms |  2.54 |  33 | 1909.9 |  349.5 |       7406 B |        7.55 |
| ReadLine_        | Row   | 50000   |    15.115 ms |  2.21 |  33 | 2208.2 |  302.3 |  111389433 B |  113,546.82 |
| CsvHelper        | Row   | 50000   |    52.863 ms |  7.68 |  33 |  631.4 | 1057.3 |      21081 B |       21.49 |
|                  |       |         |              |       |     |        |        |              |             |
| Sep______        | Cols  | 50000   |     7.683 ms |  1.00 |  33 | 4344.4 |  153.7 |        984 B |        1.00 |
| Sep_Unescape     | Cols  | 50000   |     8.728 ms |  1.14 |  33 | 3824.3 |  174.6 |        987 B |        1.00 |
| Sylvan___        | Cols  | 50000   |    20.225 ms |  2.63 |  33 | 1650.3 |  404.5 |       7411 B |        7.53 |
| ReadLine_        | Cols  | 50000   |    15.128 ms |  1.97 |  33 | 2206.4 |  302.6 |  111389433 B |  113,200.64 |
| CsvHelper        | Cols  | 50000   |    82.537 ms | 10.74 |  33 |  404.4 | 1650.7 |     457060 B |      464.49 |
|                  |       |         |              |       |     |        |        |              |             |
| Sep______        | Asset | 1000000 |   790.268 ms |  1.00 | 667 |  844.9 |  790.3 |  273121432 B |        1.00 |
| Sep_Unescape     | Asset | 1000000 |   756.227 ms |  0.96 | 667 |  882.9 |  756.2 |  273067384 B |        1.00 |
| Sep__MT__        | Asset | 1000000 |   403.149 ms |  0.51 | 667 | 1656.2 |  403.1 |  274175488 B |        1.00 |
| Sep__MT_Unescape | Asset | 1000000 |   377.297 ms |  0.49 | 667 | 1769.7 |  377.3 |  274817976 B |        1.01 |
| Sylvan___        | Asset | 1000000 | 1,077.943 ms |  1.36 | 667 |  619.4 | 1077.9 |  273233704 B |        1.00 |
| ReadLine_        | Asset | 1000000 | 2,887.799 ms |  3.71 | 667 |  231.2 | 2887.8 | 2500932168 B |        9.16 |
| CsvHelper        | Asset | 1000000 | 2,094.822 ms |  2.66 | 667 |  318.7 | 2094.8 |  273236496 B |        1.00 |
