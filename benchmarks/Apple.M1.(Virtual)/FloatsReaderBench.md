```

BenchmarkDotNet v0.15.6, macOS Sequoia 15.7.1 (24G231) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
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
| Sep______ | Row    | 25000 |   3.546 ms |  1.00 | 20 | 5716.9 |  141.8 |     1.16 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  21.472 ms |  6.08 | 20 |  944.1 |  858.9 |    10.36 KB |        8.90 |
| ReadLine_ | Row    | 25000 |  19.918 ms |  5.64 | 20 | 1017.8 |  796.7 | 73489.62 KB |   63,132.02 |
| CsvHelper | Row    | 25000 |  37.422 ms | 10.59 | 20 |  541.7 | 1496.9 |    19.95 KB |       17.13 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   3.986 ms |  1.01 | 20 | 5085.7 |  159.4 |     1.16 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  24.931 ms |  6.29 | 20 |  813.1 |  997.2 |    10.52 KB |        9.04 |
| ReadLine_ | Cols   | 25000 |  17.889 ms |  4.51 | 20 | 1133.2 |  715.6 | 73489.62 KB |   63,132.02 |
| CsvHelper | Cols   | 25000 |  47.480 ms | 11.98 | 20 |  427.0 | 1899.2 | 21340.16 KB |   18,332.49 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  34.253 ms |  1.02 | 20 |  591.8 | 1370.1 |     7.81 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  17.345 ms |  0.52 | 20 | 1168.7 |  693.8 |   101.17 KB |       12.95 |
| Sylvan___ | Floats | 25000 |  87.013 ms |  2.60 | 20 |  233.0 | 3480.5 |    18.31 KB |        2.34 |
| ReadLine_ | Floats | 25000 | 101.080 ms |  3.01 | 20 |  200.6 | 4043.2 | 73492.94 KB |    9,407.10 |
| CsvHelper | Floats | 25000 | 116.872 ms |  3.49 | 20 |  173.5 | 4674.9 | 22061.55 KB |    2,823.88 |
