```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26200.6899) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.295 ms |  1.00 | 20 | 6166.2 |  131.8 |     1.16 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  23.661 ms |  7.18 | 20 |  858.8 |  946.4 |    10.32 KB |        8.86 |
| ReadLine_ | Row    | 25000 |  16.952 ms |  5.14 | 20 | 1198.6 |  678.1 | 73489.62 KB |   63,132.02 |
| CsvHelper | Row    | 25000 |  33.251 ms | 10.09 | 20 |  611.1 | 1330.0 |    20.02 KB |       17.19 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.695 ms |  1.00 | 20 | 4327.6 |  187.8 |     1.16 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  26.819 ms |  5.71 | 20 |  757.6 | 1072.8 |    10.32 KB |        8.87 |
| ReadLine_ | Cols   | 25000 |  17.588 ms |  3.75 | 20 | 1155.3 |  703.5 | 73489.62 KB |   63,132.02 |
| CsvHelper | Cols   | 25000 |  36.000 ms |  7.67 | 20 |  564.4 | 1440.0 | 21340.16 KB |   18,332.49 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  29.130 ms |  1.00 | 20 |  697.6 | 1165.2 |     7.81 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   8.689 ms |  0.30 | 20 | 2338.6 |  347.6 |    86.84 KB |       11.12 |
| Sylvan___ | Floats | 25000 |  86.914 ms |  2.98 | 20 |  233.8 | 3476.6 |    18.31 KB |        2.34 |
| ReadLine_ | Floats | 25000 |  94.119 ms |  3.23 | 20 |  215.9 | 3764.7 | 73492.94 KB |    9,407.10 |
| CsvHelper | Floats | 25000 | 129.945 ms |  4.46 | 20 |  156.4 | 5197.8 | 22060.98 KB |    2,823.81 |
