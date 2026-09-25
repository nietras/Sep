```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.4 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.267 ms |  1.00 | 20 | 8941.8 |   90.7 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   2.840 ms |  1.25 | 20 | 7139.2 |  113.6 |     12.5 KB |       10.00 |
| ReadLine_ | Row    | 25000 |  13.871 ms |  6.12 | 20 | 1461.4 |  554.9 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 |  31.573 ms | 13.93 | 20 |  642.1 | 1262.9 |    19.95 KB |       15.96 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   3.218 ms |  1.00 | 20 | 6298.8 |  128.7 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   4.903 ms |  1.52 | 20 | 4134.6 |  196.1 |     12.5 KB |       10.00 |
| ReadLine_ | Cols   | 25000 |  14.131 ms |  4.39 | 20 | 1434.5 |  565.3 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 |  34.032 ms | 10.57 | 20 |  595.7 | 1361.3 | 21340.16 KB |   17,072.13 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  26.473 ms |  1.00 | 20 |  765.8 | 1058.9 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  11.986 ms |  0.45 | 20 | 1691.3 |  479.4 |    76.16 KB |        9.64 |
| Sylvan___ | Floats | 25000 |  64.253 ms |  2.43 | 20 |  315.5 | 2570.1 |    18.62 KB |        2.36 |
| ReadLine_ | Floats | 25000 |  88.300 ms |  3.34 | 20 |  229.6 | 3532.0 | 73492.93 KB |    9,304.74 |
| CsvHelper | Floats | 25000 | 123.955 ms |  4.68 | 20 |  163.5 | 4958.2 | 22061.55 KB |    2,793.15 |
