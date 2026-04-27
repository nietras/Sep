```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 3.24GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.029 ms |  1.00 | 20 | 6693.5 |  121.1 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.453 ms |  1.14 | 20 | 5871.1 |  138.1 |     12.5 KB |       10.00 |
| ReadLine_ | Row    | 25000 |  18.866 ms |  6.23 | 20 | 1074.5 |  754.7 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 |  37.742 ms | 12.46 | 20 |  537.1 | 1509.7 |    19.95 KB |       15.96 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.128 ms |  1.00 | 20 | 4910.3 |  165.1 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   5.953 ms |  1.44 | 20 | 3405.5 |  238.1 |     12.5 KB |       10.00 |
| ReadLine_ | Cols   | 25000 |  18.656 ms |  4.52 | 20 | 1086.6 |  746.3 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 |  40.471 ms |  9.80 | 20 |  500.9 | 1618.9 | 21340.16 KB |   17,072.13 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  33.781 ms |  1.00 | 20 |  600.1 | 1351.3 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  15.381 ms |  0.46 | 20 | 1318.0 |  615.2 |    70.64 KB |        8.94 |
| Sylvan___ | Floats | 25000 |  80.037 ms |  2.37 | 20 |  253.3 | 3201.5 |    21.64 KB |        2.74 |
| ReadLine_ | Floats | 25000 | 117.401 ms |  3.48 | 20 |  172.7 | 4696.0 | 73492.94 KB |    9,304.74 |
| CsvHelper | Floats | 25000 | 156.597 ms |  4.64 | 20 |  129.5 | 6263.9 | 22061.51 KB |    2,793.15 |
