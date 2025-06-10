```

BenchmarkDotNet v0.15.1, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-HVKXWJ : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-HVKXWJ  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    30.88 ms |  1.01 |  29 |  942.0 |  617.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    17.27 ms |  0.57 |  29 | 1683.8 |  345.5 |   13.63 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    58.00 ms |  1.91 |  29 |  501.5 | 1160.0 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    57.52 ms |  1.89 |  29 |  505.6 | 1150.5 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    92.72 ms |  3.05 |  29 |  313.7 | 1854.4 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   464.62 ms |  1.00 | 581 | 1252.4 |  464.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   235.56 ms |  0.51 | 581 | 2470.2 |  235.6 |  270.36 MB |        1.04 |
| Sylvan___ | Asset | 1000000 | 1,039.54 ms |  2.24 | 581 |  559.7 | 1039.5 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,041.60 ms |  2.24 | 581 |  558.6 | 1041.6 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,516.21 ms |  3.26 | 581 |  383.8 | 1516.2 |  260.58 MB |        1.00 |
