```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2
  Job-LKXTKX : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |  1.909 ms |  1.00 | 20 | 10643.3 |   76.4 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  2.244 ms |  1.18 | 20 |  9054.3 |   89.8 |     10.7 KB |        8.56 |
| ReadLine_ | Row    | 25000 |  9.875 ms |  5.17 | 20 |  2057.8 |  395.0 | 73489.63 KB |   58,791.71 |
| CsvHelper | Row    | 25000 | 24.433 ms | 12.80 | 20 |   831.6 |  977.3 |       20 KB |       16.00 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |  2.615 ms |  1.00 | 20 |  7771.6 |  104.6 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  3.744 ms |  1.43 | 20 |  5427.2 |  149.8 |    10.71 KB |        8.54 |
| ReadLine_ | Cols   | 25000 | 10.169 ms |  3.89 | 20 |  1998.1 |  406.8 | 73489.63 KB |   58,654.23 |
| CsvHelper | Cols   | 25000 | 26.080 ms |  9.97 | 20 |   779.1 | 1043.2 | 21340.22 KB |   17,032.26 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Floats | 25000 | 19.885 ms |  1.00 | 20 |  1021.8 |  795.4 |     7.99 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  3.527 ms |  0.18 | 20 |  5761.0 |  141.1 |   180.36 KB |       22.59 |
| Sylvan___ | Floats | 25000 | 52.686 ms |  2.65 | 20 |   385.7 | 2107.5 |    18.88 KB |        2.36 |
| ReadLine_ | Floats | 25000 | 66.897 ms |  3.36 | 20 |   303.7 | 2675.9 | 73493.12 KB |    9,203.49 |
| CsvHelper | Floats | 25000 | 97.448 ms |  4.90 | 20 |   208.5 | 3897.9 |  22061.7 KB |    2,762.77 |
