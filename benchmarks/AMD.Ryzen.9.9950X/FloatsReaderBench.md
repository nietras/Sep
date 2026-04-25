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
| Sep______ | Row    | 25000 |  1.271 ms |  1.00 | 20 | 15981.3 |   50.9 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1.676 ms |  1.32 | 20 | 12120.9 |   67.1 |     12.5 KB |       10.00 |
| ReadLine_ | Row    | 25000 |  6.881 ms |  5.41 | 20 |  2952.8 |  275.3 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 | 15.235 ms | 11.98 | 20 |  1333.7 |  609.4 |    19.95 KB |       15.96 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |  2.019 ms |  1.00 | 20 | 10063.8 |   80.8 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2.675 ms |  1.32 | 20 |  7597.0 |  107.0 |     12.5 KB |       10.00 |
| ReadLine_ | Cols   | 25000 |  7.251 ms |  3.59 | 20 |  2802.5 |  290.0 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 | 16.365 ms |  8.11 | 20 |  1241.6 |  654.6 | 21340.16 KB |   17,072.13 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Floats | 25000 | 16.987 ms |  1.00 | 20 |  1196.2 |  679.5 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  2.108 ms |  0.12 | 20 |  9637.3 |   84.3 |   180.28 KB |       22.82 |
| Sylvan___ | Floats | 25000 | 35.994 ms |  2.12 | 20 |   564.5 | 1439.8 |     18.6 KB |        2.35 |
| ReadLine_ | Floats | 25000 | 48.351 ms |  2.85 | 20 |   420.3 | 1934.0 | 73492.94 KB |    9,304.74 |
| CsvHelper | Floats | 25000 | 67.877 ms |  4.00 | 20 |   299.4 | 2715.1 | 22061.22 KB |    2,793.11 |
