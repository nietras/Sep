```

BenchmarkDotNet v0.13.10, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-TUSNIO : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-TUSNIO  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=True  
Reader=String  

```
| Method       | Scope | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000 |   7.453 ms |  1.00 | 33 | 4478.2 |  149.1 |       944 B |        1.00 |
| Sep_Unescape | Row   | 50000 |   7.381 ms |  0.99 | 33 | 4522.2 |  147.6 |       944 B |        1.00 |
| Sylvan___    | Row   | 50000 |  18.035 ms |  2.40 | 33 | 1850.7 |  360.7 |      7390 B |        7.83 |
| ReadLine_    | Row   | 50000 |  14.689 ms |  1.97 | 33 | 2272.3 |  293.8 | 111389433 B |  117,997.28 |
| CsvHelper    | Row   | 50000 |  52.212 ms |  7.01 | 33 |  639.3 | 1044.2 |     21081 B |       22.33 |
|              |       |       |            |       |    |        |        |             |             |
| Sep______    | Cols  | 50000 |   7.780 ms |  1.00 | 33 | 4290.1 |  155.6 |       946 B |        1.00 |
| Sep_Unescape | Cols  | 50000 |   8.337 ms |  1.07 | 33 | 4003.6 |  166.7 |       946 B |        1.00 |
| Sylvan___    | Cols  | 50000 |  20.329 ms |  2.60 | 33 | 1641.9 |  406.6 |      7411 B |        7.83 |
| ReadLine_    | Cols  | 50000 |  15.540 ms |  1.99 | 33 | 2147.8 |  310.8 | 111389433 B |  117,747.82 |
| CsvHelper    | Cols  | 50000 |  83.149 ms | 10.64 | 33 |  401.4 | 1663.0 |    457060 B |      483.15 |
|              |       |       |            |       |    |        |        |             |             |
| Sep______    | Asset | 50000 |  35.903 ms |  1.00 | 33 |  929.6 |  718.1 |  14139438 B |        1.00 |
| Sep_Unescape | Asset | 50000 |  34.316 ms |  0.93 | 33 |  972.7 |  686.3 |  14130926 B |        1.00 |
| Sylvan___    | Asset | 50000 |  51.465 ms |  1.43 | 33 |  648.5 | 1029.3 |  14296613 B |        1.01 |
| ReadLine_    | Asset | 50000 | 117.914 ms |  3.29 | 33 |  283.1 | 2358.3 | 125239144 B |        8.86 |
| CsvHelper    | Asset | 50000 |  96.670 ms |  2.57 | 33 |  345.3 | 1933.4 |  14307304 B |        1.01 |
