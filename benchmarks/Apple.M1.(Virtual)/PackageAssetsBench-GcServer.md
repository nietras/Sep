```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.2 (23H311) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD
  Job-EAJOLX : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD

Job=Job-EAJOLX  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    24.49 ms |  1.00 |  29 | 1187.5 |  489.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    10.69 ms |  0.44 |  29 | 2720.6 |  213.8 |   13.61 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    47.47 ms |  1.94 |  29 |  612.7 |  949.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    41.20 ms |  1.68 |  29 |  705.9 |  824.0 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    73.98 ms |  3.02 |  29 |  393.1 | 1479.7 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   491.52 ms |  1.00 | 581 | 1183.8 |  491.5 |  260.43 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   207.37 ms |  0.42 | 581 | 2806.0 |  207.4 |  266.72 MB |        1.02 |
| Sylvan___ | Asset | 1000000 |   960.79 ms |  1.95 | 581 |  605.6 |  960.8 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,008.81 ms |  2.05 | 581 |  576.8 | 1008.8 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,504.22 ms |  3.06 | 581 |  386.8 | 1504.2 |  260.58 MB |        1.00 |
