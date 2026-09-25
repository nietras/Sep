```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.34 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.954 ms |  1.00 | 20 | 6863.4 |  118.1 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.551 ms |  1.20 | 20 | 5708.1 |  142.1 |     12.5 KB |       10.00 |
| ReadLine_ | Row    | 25000 |  17.379 ms |  5.88 | 20 | 1166.5 |  695.2 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 |  40.075 ms | 13.57 | 20 |  505.9 | 1603.0 |    19.95 KB |       15.96 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.123 ms |  1.00 | 20 | 4917.0 |  164.9 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.166 ms |  1.50 | 20 | 3287.8 |  246.6 |     12.5 KB |       10.00 |
| ReadLine_ | Cols   | 25000 |  17.507 ms |  4.25 | 20 | 1158.0 |  700.3 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 |  43.418 ms | 10.53 | 20 |  466.9 | 1736.7 | 21340.16 KB |   17,072.13 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  34.638 ms |  1.00 | 20 |  585.3 | 1385.5 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  15.520 ms |  0.45 | 20 | 1306.2 |  620.8 |    72.32 KB |        9.16 |
| Sylvan___ | Floats | 25000 |  84.455 ms |  2.44 | 20 |  240.0 | 3378.2 |    18.63 KB |        2.36 |
| ReadLine_ | Floats | 25000 | 117.412 ms |  3.39 | 20 |  172.7 | 4696.5 | 73492.93 KB |    9,304.74 |
| CsvHelper | Floats | 25000 | 159.867 ms |  4.62 | 20 |  126.8 | 6394.7 | 22061.55 KB |    2,793.15 |
