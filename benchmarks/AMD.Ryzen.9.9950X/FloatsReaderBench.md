```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7184/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |  1.277 ms |  1.00 | 20 | 15906.0 |   51.1 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1.674 ms |  1.31 | 20 | 12134.9 |   67.0 |     12.5 KB |       10.00 |
| ReadLine_ | Row    | 25000 |  6.856 ms |  5.37 | 20 |  2963.9 |  274.2 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 | 15.119 ms | 11.84 | 20 |  1344.0 |  604.8 |    19.95 KB |       15.96 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |  2.019 ms |  1.00 | 20 | 10061.9 |   80.8 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2.671 ms |  1.32 | 20 |  7608.9 |  106.8 |     12.5 KB |       10.00 |
| ReadLine_ | Cols   | 25000 |  7.217 ms |  3.57 | 20 |  2815.5 |  288.7 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 | 16.284 ms |  8.06 | 20 |  1247.8 |  651.4 | 21340.17 KB |   17,072.13 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Floats | 25000 | 15.878 ms |  1.00 | 20 |  1279.7 |  635.1 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  2.140 ms |  0.13 | 20 |  9493.1 |   85.6 |   180.97 KB |       22.90 |
| Sylvan___ | Floats | 25000 | 36.212 ms |  2.28 | 20 |   561.1 | 1448.5 |     18.6 KB |        2.35 |
| ReadLine_ | Floats | 25000 | 48.538 ms |  3.06 | 20 |   418.6 | 1941.5 | 73492.94 KB |    9,301.29 |
| CsvHelper | Floats | 25000 | 68.256 ms |  4.30 | 20 |   297.7 | 2730.2 | 22061.22 KB |    2,792.08 |
