```

BenchmarkDotNet v0.15.1, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-SNAXSU : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-SNAXSU  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    31.85 ms |  1.00 |  33 | 1045.1 |  636.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    12.27 ms |  0.39 |  33 | 2712.4 |  245.4 |    13.6 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    49.61 ms |  1.56 |  33 |  670.9 |  992.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    40.30 ms |  1.27 |  33 |  825.9 |  805.9 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    91.88 ms |  2.89 |  33 |  362.2 | 1837.6 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   803.91 ms |  1.01 | 665 |  828.2 |  803.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   267.90 ms |  0.34 | 665 | 2485.3 |  267.9 |  265.71 MB |        1.02 |
| Sylvan___ | Asset | 1000000 |   984.68 ms |  1.24 | 665 |  676.2 |  984.7 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,483.17 ms |  1.87 | 665 |  448.9 | 1483.2 | 2385.08 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,595.39 ms |  2.01 | 665 |  417.3 | 1595.4 |  260.58 MB |        1.00 |
