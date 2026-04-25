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
| Sep______ | Row    | 25000 |  1.270 ms |  1.00 | 20 | 16004.0 |   50.8 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1.691 ms |  1.33 | 20 | 12018.0 |   67.6 |     12.5 KB |       10.00 |
| ReadLine_ | Row    | 25000 |  6.893 ms |  5.43 | 20 |  2948.0 |  275.7 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 | 14.958 ms | 11.78 | 20 |  1358.4 |  598.3 |    19.95 KB |       15.96 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |  1.849 ms |  1.00 | 20 | 10991.8 |   73.9 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2.651 ms |  1.43 | 20 |  7665.7 |  106.0 |     12.5 KB |       10.00 |
| ReadLine_ | Cols   | 25000 |  7.216 ms |  3.90 | 20 |  2815.8 |  288.7 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 | 16.252 ms |  8.79 | 20 |  1250.3 |  650.1 | 21340.17 KB |   17,072.13 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Floats | 25000 | 16.656 ms |  1.00 | 20 |  1220.0 |  666.2 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  2.063 ms |  0.12 | 20 |  9851.1 |   82.5 |   180.33 KB |       22.83 |
| Sylvan___ | Floats | 25000 | 35.401 ms |  2.13 | 20 |   574.0 | 1416.1 |     18.6 KB |        2.35 |
| ReadLine_ | Floats | 25000 | 47.950 ms |  2.88 | 20 |   423.8 | 1918.0 | 73492.94 KB |    9,304.74 |
| CsvHelper | Floats | 25000 | 68.322 ms |  4.10 | 20 |   297.4 | 2732.9 | 22061.22 KB |    2,793.11 |
