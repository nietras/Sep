```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-PQAZLF : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Runtime=.NET 8.0  Toolchain=net80  InvocationCount=Default  
IterationTime=350.0000 ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   1.936 ms |  1.00 | 20 | 10498.4 |   77.4 |     1.18 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   2.259 ms |  1.17 | 20 |  8995.4 |   90.4 |    10.02 KB |        8.50 |
| ReadLine_ | Row    | 25000 |  11.063 ms |  5.69 | 20 |  1836.8 |  442.5 | 73489.64 KB |   62,295.85 |
| CsvHelper | Row    | 25000 |  25.135 ms | 12.99 | 20 |   808.4 | 1005.4 |    20.58 KB |       17.45 |
|           |        |       |            |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |   2.566 ms |  1.00 | 20 |  7918.8 |  102.6 |     1.18 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   3.821 ms |  1.49 | 20 |  5318.1 |  152.8 |    10.03 KB |        8.48 |
| ReadLine_ | Cols   | 25000 |  11.675 ms |  4.54 | 20 |  1740.5 |  467.0 | 73489.64 KB |   62,141.53 |
| CsvHelper | Cols   | 25000 |  27.400 ms | 10.67 | 20 |   741.6 | 1096.0 | 21340.81 KB |   18,045.40 |
|           |        |       |            |       |    |         |        |             |             |
| Sep______ | Floats | 25000 |  21.852 ms |  1.00 | 20 |   929.9 |  874.1 |     7.93 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   3.916 ms |  0.18 | 20 |  5188.4 |  156.7 |   178.87 KB |       22.56 |
| Sylvan___ | Floats | 25000 |  71.861 ms |  3.29 | 20 |   282.8 | 2874.4 |     18.2 KB |        2.30 |
| ReadLine_ | Floats | 25000 |  73.981 ms |  3.39 | 20 |   274.7 | 2959.2 | 73493.12 KB |    9,271.52 |
| CsvHelper | Floats | 25000 | 102.224 ms |  4.68 | 20 |   198.8 | 4089.0 | 22062.55 KB |    2,783.30 |
