```

BenchmarkDotNet v0.15.6, macOS Sequoia 15.7.1 (24G231) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    38.27 ms |  1.02 |  33 |  869.6 |  765.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    24.23 ms |  0.65 |  33 | 1373.3 |  484.7 |   13.75 MB |        1.02 |
| Sylvan___ | Asset | 50000   |    78.94 ms |  2.11 |  33 |  421.6 | 1578.8 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    73.58 ms |  1.97 |  33 |  452.3 | 1471.6 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   127.06 ms |  3.40 |  33 |  261.9 | 2541.3 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   896.49 ms |  1.00 | 665 |  742.7 |  896.5 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   388.68 ms |  0.43 | 665 | 1713.0 |  388.7 |  271.08 MB |        1.04 |
| Sylvan___ | Asset | 1000000 | 1,703.56 ms |  1.90 | 665 |  390.8 | 1703.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 3,773.61 ms |  4.22 | 665 |  176.4 | 3773.6 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,813.28 ms |  3.14 | 665 |  236.7 | 2813.3 |  260.58 MB |        1.00 |
