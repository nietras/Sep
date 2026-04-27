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
| Sep______ | Row    | 25000 |   3.029 ms |  1.00 | 20 | 6692.8 |  121.2 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.476 ms |  1.15 | 20 | 5832.8 |  139.0 |    12.51 KB |       10.01 |
| ReadLine_ | Row    | 25000 |  18.764 ms |  6.20 | 20 | 1080.4 |  750.6 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 |  37.310 ms | 12.32 | 20 |  543.3 | 1492.4 |    19.95 KB |       15.96 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.196 ms |  1.00 | 20 | 4831.0 |  167.9 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.099 ms |  1.45 | 20 | 3324.0 |  243.9 |     12.5 KB |       10.00 |
| ReadLine_ | Cols   | 25000 |  19.189 ms |  4.57 | 20 | 1056.4 |  767.6 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 |  40.497 ms |  9.65 | 20 |  500.6 | 1619.9 | 21340.16 KB |   17,072.13 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  34.040 ms |  1.00 | 20 |  595.5 | 1361.6 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  16.374 ms |  0.48 | 20 | 1238.1 |  655.0 |    69.66 KB |        8.82 |
| Sylvan___ | Floats | 25000 |  82.025 ms |  2.41 | 20 |  247.1 | 3281.0 |    18.63 KB |        2.36 |
| ReadLine_ | Floats | 25000 | 109.538 ms |  3.22 | 20 |  185.1 | 4381.5 | 73492.94 KB |    9,304.74 |
| CsvHelper | Floats | 25000 | 154.863 ms |  4.55 | 20 |  130.9 | 6194.5 | 22061.51 KB |    2,793.15 |
