```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  Job-NGWSCM : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2

Runtime=.NET 8.0  Toolchain=net80  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   1.927 ms |  1.00 | 20 | 10544.2 |   77.1 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   2.266 ms |  1.18 | 20 |  8965.5 |   90.7 |     10.7 KB |        8.56 |
| ReadLine_ | Row    | 25000 |  10.706 ms |  5.56 | 20 |  1897.9 |  428.3 | 73489.63 KB |   58,791.71 |
| CsvHelper | Row    | 25000 |  25.310 ms | 13.13 | 20 |   802.8 | 1012.4 |       20 KB |       16.00 |
|           |        |       |            |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |   2.950 ms |  1.00 | 20 |  6889.2 |  118.0 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   3.930 ms |  1.33 | 20 |  5171.0 |  157.2 |    10.71 KB |        8.53 |
| ReadLine_ | Cols   | 25000 |  11.418 ms |  3.87 | 20 |  1779.6 |  456.7 | 73489.64 KB |   58,562.95 |
| CsvHelper | Cols   | 25000 |  27.273 ms |  9.25 | 20 |   745.1 | 1090.9 | 21340.22 KB |   17,005.75 |
|           |        |       |            |       |    |         |        |             |             |
| Sep______ | Floats | 25000 |  21.480 ms |  1.00 | 20 |   946.0 |  859.2 |     7.99 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   3.380 ms |  0.16 | 20 |  6012.2 |  135.2 |   181.49 KB |       22.71 |
| Sylvan___ | Floats | 25000 |  55.139 ms |  2.57 | 20 |   368.5 | 2205.6 |    18.88 KB |        2.36 |
| ReadLine_ | Floats | 25000 |  71.542 ms |  3.33 | 20 |   284.0 | 2861.7 | 73493.12 KB |    9,196.74 |
| CsvHelper | Floats | 25000 | 101.668 ms |  4.73 | 20 |   199.9 | 4066.7 | 22061.92 KB |    2,760.77 |
