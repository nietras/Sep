```

BenchmarkDotNet v0.15.6, macOS Sequoia 15.7.1 (24G231) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    31.64 ms |  1.01 |  29 |  919.3 |  632.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    23.46 ms |  0.75 |  29 | 1239.8 |  469.2 |   13.65 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    61.59 ms |  1.96 |  29 |  472.2 | 1231.8 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    53.87 ms |  1.71 |  29 |  539.9 | 1077.4 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   109.28 ms |  3.48 |  29 |  266.2 | 2185.6 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   689.69 ms |  1.00 | 581 |  843.7 |  689.7 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   311.19 ms |  0.45 | 581 | 1869.8 |  311.2 |  270.88 MB |        1.04 |
| Sylvan___ | Asset | 1000000 | 1,380.63 ms |  2.01 | 581 |  421.5 | 1380.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 2,073.15 ms |  3.02 | 581 |  280.7 | 2073.1 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,448.67 ms |  3.57 | 581 |  237.6 | 2448.7 |  260.58 MB |        1.00 |
