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
| Sep______ | Row    | 25000 |  1.254 ms |  1.00 | 20 | 16201.2 |   50.2 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1.674 ms |  1.33 | 20 | 12136.0 |   67.0 |     12.5 KB |       10.00 |
| ReadLine_ | Row    | 25000 |  6.880 ms |  5.49 | 20 |  2953.6 |  275.2 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 | 15.116 ms | 12.05 | 20 |  1344.3 |  604.6 |    19.95 KB |       15.96 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |  2.038 ms |  1.00 | 20 |  9969.3 |   81.5 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2.680 ms |  1.32 | 20 |  7582.1 |  107.2 |     12.5 KB |       10.00 |
| ReadLine_ | Cols   | 25000 |  7.242 ms |  3.56 | 20 |  2805.9 |  289.7 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 | 16.322 ms |  8.02 | 20 |  1244.9 |  652.9 | 21340.17 KB |   17,072.13 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Floats | 25000 | 16.569 ms |  1.00 | 20 |  1226.3 |  662.8 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  2.101 ms |  0.13 | 20 |  9671.6 |   84.0 |   180.09 KB |       22.80 |
| Sylvan___ | Floats | 25000 | 36.068 ms |  2.18 | 20 |   563.4 | 1442.7 |     18.6 KB |        2.35 |
| ReadLine_ | Floats | 25000 | 48.812 ms |  2.95 | 20 |   416.3 | 1952.5 | 73492.94 KB |    9,304.74 |
| CsvHelper | Floats | 25000 | 67.545 ms |  4.08 | 20 |   300.8 | 2701.8 | 22061.22 KB |    2,793.11 |
