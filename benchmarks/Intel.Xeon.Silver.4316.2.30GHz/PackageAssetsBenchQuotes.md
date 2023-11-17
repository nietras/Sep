```

BenchmarkDotNet v0.13.10, Windows 10 (10.0.17763.3287/1809/October2018Update/Redstone5)
Intel Xeon Silver 4316 CPU 2.30GHz, 1 CPU, 40 logical and 20 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-ETZYVC : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-ETZYVC  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=True  
Reader=String  

```
| Method       | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |------ |----------:|------:|---:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000 |  13.63 ms |  1.00 | 33 | 2449.1 |  272.6 |      1.09 KB |        1.00 |
| Sep_Unescape | Row   | 50000 |  13.32 ms |  0.98 | 33 | 2505.9 |  266.4 |      1.09 KB |        1.00 |
| Sylvan___    | Row   | 50000 |  34.82 ms |  2.58 | 33 |  958.6 |  696.4 |      7.26 KB |        6.66 |
| ReadLine_    | Row   | 50000 |  31.35 ms |  2.30 | 33 | 1064.6 |  627.1 | 108778.77 KB |   99,721.99 |
| CsvHelper    | Row   | 50000 | 103.98 ms |  7.63 | 33 |  321.0 | 2079.6 |     20.69 KB |       18.97 |
|              |       |       |           |       |    |        |        |              |             |
| Sep______    | Cols  | 50000 |  15.17 ms |  1.00 | 33 | 2199.6 |  303.5 |      1.09 KB |        1.00 |
| Sep_Unescape | Cols  | 50000 |  16.70 ms |  1.10 | 33 | 1998.6 |  334.0 |       1.1 KB |        1.00 |
| Sylvan___    | Cols  | 50000 |  38.12 ms |  2.51 | 33 |  875.7 |  762.3 |      7.28 KB |        6.65 |
| ReadLine_    | Cols  | 50000 |  31.59 ms |  2.06 | 33 | 1056.4 |  631.9 | 108778.77 KB |   99,454.88 |
| CsvHelper    | Cols  | 50000 | 162.58 ms | 10.71 | 33 |  205.3 | 3251.7 |     446.3 KB |      408.05 |
|              |       |       |           |       |    |        |        |              |             |
| Sep______    | Asset | 50000 |  68.20 ms |  1.00 | 33 |  489.4 | 1364.1 |  13808.63 KB |        1.00 |
| Sep_Unescape | Asset | 50000 |  66.68 ms |  0.98 | 33 |  500.5 | 1333.6 |  13799.99 KB |        1.00 |
| Sylvan___    | Asset | 50000 |  97.10 ms |  1.43 | 33 |  343.7 | 1942.1 |  13962.36 KB |        1.01 |
| ReadLine_    | Asset | 50000 | 187.07 ms |  2.67 | 33 |  178.4 | 3741.4 | 122304.57 KB |        8.86 |
| CsvHelper    | Asset | 50000 | 181.59 ms |  2.67 | 33 |  183.8 | 3631.8 |   13970.8 KB |        1.01 |
