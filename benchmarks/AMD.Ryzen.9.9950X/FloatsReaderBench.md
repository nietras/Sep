```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 9950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-RXSQJG : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |  1.371 ms |  1.00 | 20 | 14824.5 |   54.8 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1.715 ms |  1.25 | 20 | 11845.1 |   68.6 |     10.7 KB |        8.59 |
| ReadLine_ | Row    | 25000 |  7.060 ms |  5.15 | 20 |  2878.2 |  282.4 | 73489.63 KB |   58,976.01 |
| CsvHelper | Row    | 25000 | 15.281 ms | 11.15 | 20 |  1329.7 |  611.2 |    19.96 KB |       16.02 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |  1.764 ms |  1.00 | 20 | 11520.0 |   70.6 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2.675 ms |  1.52 | 20 |  7596.7 |  107.0 |     10.7 KB |        8.59 |
| ReadLine_ | Cols   | 25000 |  8.170 ms |  4.63 | 20 |  2487.0 |  326.8 | 73489.63 KB |   58,976.01 |
| CsvHelper | Cols   | 25000 | 16.694 ms |  9.47 | 20 |  1217.2 |  667.8 |  21340.2 KB |   17,125.68 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Floats | 25000 | 16.182 ms |  1.00 | 20 |  1255.7 |  647.3 |     7.94 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  2.497 ms |  0.15 | 20 |  8136.8 |   99.9 |   179.81 KB |       22.64 |
| Sylvan___ | Floats | 25000 | 38.800 ms |  2.40 | 20 |   523.7 | 1552.0 |    18.72 KB |        2.36 |
| ReadLine_ | Floats | 25000 | 54.117 ms |  3.34 | 20 |   375.5 | 2164.7 | 73493.05 KB |    9,253.27 |
| CsvHelper | Floats | 25000 | 71.601 ms |  4.42 | 20 |   283.8 | 2864.1 | 22061.55 KB |    2,777.70 |
