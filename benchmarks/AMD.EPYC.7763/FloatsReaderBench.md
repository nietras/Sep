```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 3.08GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.38 GB Available
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
| Sep______ | Row    | 25000 |   3.029 ms |  1.00 | 20 | 6693.3 |  121.1 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.505 ms |  1.16 | 20 | 5783.9 |  140.2 |     12.5 KB |       10.00 |
| ReadLine_ | Row    | 25000 |  17.985 ms |  5.94 | 20 | 1127.2 |  719.4 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 |  38.667 ms | 12.77 | 20 |  524.3 | 1546.7 |    19.95 KB |       15.96 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.166 ms |  1.00 | 20 | 4865.7 |  166.7 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.018 ms |  1.44 | 20 | 3368.5 |  240.7 |     12.5 KB |       10.00 |
| ReadLine_ | Cols   | 25000 |  17.561 ms |  4.21 | 20 | 1154.4 |  702.4 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 |  40.924 ms |  9.82 | 20 |  495.4 | 1637.0 | 21340.16 KB |   17,072.13 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  33.194 ms |  1.00 | 20 |  610.7 | 1327.8 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  15.160 ms |  0.46 | 20 | 1337.2 |  606.4 |       73 KB |        9.24 |
| Sylvan___ | Floats | 25000 |  89.559 ms |  2.70 | 20 |  226.4 | 3582.4 |    18.63 KB |        2.36 |
| ReadLine_ | Floats | 25000 | 116.923 ms |  3.52 | 20 |  173.4 | 4676.9 | 73492.93 KB |    9,304.74 |
| CsvHelper | Floats | 25000 | 157.177 ms |  4.74 | 20 |  129.0 | 6287.1 | 22061.55 KB |    2,793.15 |
