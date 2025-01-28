```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.2 (23H311) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD
  Job-FNIGBL : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD

Job=Job-FNIGBL  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    26.05 ms |  1.00 |  29 | 1116.6 |  521.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.03 ms |  0.42 |  29 | 2636.5 |  220.6 |   13.59 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    48.67 ms |  1.87 |  29 |  597.6 |  973.4 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    34.94 ms |  1.34 |  29 |  832.4 |  698.9 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    73.10 ms |  2.81 |  29 |  397.9 | 1461.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   507.79 ms |  1.00 | 581 | 1145.9 |  507.8 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   204.22 ms |  0.40 | 581 | 2849.3 |  204.2 |  269.28 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   991.41 ms |  1.95 | 581 |  586.9 |  991.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,083.07 ms |  2.13 | 581 |  537.2 | 1083.1 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,924.79 ms |  3.79 | 581 |  302.3 | 1924.8 |  260.58 MB |        1.00 |
