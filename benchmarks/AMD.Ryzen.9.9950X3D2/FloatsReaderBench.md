```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 9950X3D2 4.30GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 52.15 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |  1.347 ms |  1.00 | 20 | 15084.0 |   53.9 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1.680 ms |  1.25 | 20 | 12097.9 |   67.2 |     12.5 KB |       10.00 |
| ReadLine_ | Row    | 25000 |  6.227 ms |  4.62 | 20 |  3263.0 |  249.1 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 | 15.297 ms | 11.36 | 20 |  1328.3 |  611.9 |    19.95 KB |       15.96 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |  1.951 ms |  1.00 | 20 | 10415.9 |   78.0 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2.614 ms |  1.34 | 20 |  7772.9 |  104.6 |     12.5 KB |       10.00 |
| ReadLine_ | Cols   | 25000 |  6.292 ms |  3.23 | 20 |  3229.5 |  251.7 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 | 16.238 ms |  8.32 | 20 |  1251.4 |  649.5 | 21340.16 KB |   17,072.13 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Floats | 25000 | 16.401 ms |  1.00 | 20 |  1238.9 |  656.0 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  2.337 ms |  0.14 | 20 |  8694.0 |   93.5 |   179.75 KB |       22.76 |
| Sylvan___ | Floats | 25000 | 37.061 ms |  2.26 | 20 |   548.3 | 1482.4 |     18.6 KB |        2.35 |
| ReadLine_ | Floats | 25000 | 49.446 ms |  3.01 | 20 |   410.9 | 1977.9 | 73492.93 KB |    9,304.74 |
| CsvHelper | Floats | 25000 | 69.442 ms |  4.23 | 20 |   292.6 | 2777.7 | 22060.97 KB |    2,793.08 |
