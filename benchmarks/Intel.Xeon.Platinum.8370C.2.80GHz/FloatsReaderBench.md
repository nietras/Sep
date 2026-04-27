```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 2.56GHz), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.751 ms |  1.00 | 20 | 5404.2 |  150.0 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.771 ms |  1.01 | 20 | 5376.1 |  150.8 |     12.5 KB |       10.00 |
| ReadLine_ | Row    | 25000 |  20.859 ms |  5.56 | 20 |  971.8 |  834.4 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 |  44.841 ms | 11.96 | 20 |  452.1 | 1793.6 |    19.95 KB |       15.96 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   5.197 ms |  1.00 | 20 | 3900.8 |  207.9 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.571 ms |  1.26 | 20 | 3085.3 |  262.8 |     12.5 KB |       10.00 |
| ReadLine_ | Cols   | 25000 |  21.002 ms |  4.04 | 20 |  965.3 |  840.1 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 |  47.996 ms |  9.24 | 20 |  422.4 | 1919.8 | 21340.16 KB |   17,072.13 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  35.863 ms |  1.00 | 20 |  565.3 | 1434.5 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  22.056 ms |  0.62 | 20 |  919.1 |  882.2 |    68.48 KB |        8.67 |
| Sylvan___ | Floats | 25000 |  76.947 ms |  2.15 | 20 |  263.5 | 3077.9 |    18.63 KB |        2.36 |
| ReadLine_ | Floats | 25000 | 109.967 ms |  3.07 | 20 |  184.3 | 4398.7 | 73492.94 KB |    9,304.74 |
| CsvHelper | Floats | 25000 | 153.602 ms |  4.28 | 20 |  132.0 | 6144.1 | 22061.27 KB |    2,793.12 |
