```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 9950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.103
  [Host]     : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-MIRFZN : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |  1.247 ms |  1.00 | 20 | 16290.5 |   49.9 |     1.45 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1.617 ms |  1.30 | 20 | 12565.6 |   64.7 |     10.7 KB |        7.40 |
| ReadLine_ | Row    | 25000 |  6.965 ms |  5.58 | 20 |  2917.5 |  278.6 | 73489.63 KB |   50,812.55 |
| CsvHelper | Row    | 25000 | 14.923 ms | 11.96 | 20 |  1361.7 |  596.9 |    19.98 KB |       13.81 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |  1.682 ms |  1.00 | 20 | 12078.3 |   67.3 |     1.41 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2.404 ms |  1.43 | 20 |  8452.3 |   96.2 |     10.7 KB |        7.62 |
| ReadLine_ | Cols   | 25000 |  7.444 ms |  4.42 | 20 |  2729.9 |  297.7 | 73489.63 KB |   52,295.61 |
| CsvHelper | Cols   | 25000 | 16.345 ms |  9.72 | 20 |  1243.2 |  653.8 |  21340.2 KB |   15,185.80 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Floats | 25000 | 15.559 ms |  1.00 | 20 |  1305.9 |  622.4 |     8.65 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  2.365 ms |  0.15 | 20 |  8591.2 |   94.6 |      181 KB |       20.93 |
| Sylvan___ | Floats | 25000 | 36.757 ms |  2.36 | 20 |   552.8 | 1470.3 |    18.72 KB |        2.16 |
| ReadLine_ | Floats | 25000 | 50.909 ms |  3.27 | 20 |   399.1 | 2036.4 | 73493.06 KB |    8,499.76 |
| CsvHelper | Floats | 25000 | 69.428 ms |  4.46 | 20 |   292.7 | 2777.1 | 22061.69 KB |    2,551.52 |
