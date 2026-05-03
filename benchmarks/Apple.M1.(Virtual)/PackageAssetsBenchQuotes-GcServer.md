```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.03 ms |  1.01 |  33 | 1039.2 |  640.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    25.90 ms |  0.82 |  33 | 1285.2 |  517.9 |   13.59 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    64.43 ms |  2.03 |  33 |  516.5 | 1288.7 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    51.58 ms |  1.63 |  33 |  645.2 | 1031.7 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    85.12 ms |  2.68 |  33 |  391.0 | 1702.4 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   803.40 ms |  1.01 | 665 |  828.7 |  803.4 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   364.58 ms |  0.46 | 665 | 1826.2 |  364.6 |  271.19 MB |        1.04 |
| Sylvan___ | Asset | 1000000 | 1,311.83 ms |  1.64 | 665 |  507.5 | 1311.8 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 2,342.81 ms |  2.94 | 665 |  284.2 | 2342.8 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,052.00 ms |  2.57 | 665 |  324.5 | 2052.0 |  260.58 MB |        1.00 |
