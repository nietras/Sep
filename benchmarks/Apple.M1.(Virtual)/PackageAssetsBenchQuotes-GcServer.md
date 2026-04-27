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
| Sep______ | Asset | 50000   |    29.12 ms |  1.00 |  33 | 1143.1 |  582.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    12.10 ms |  0.42 |  33 | 2751.6 |  241.9 |   13.66 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    52.81 ms |  1.82 |  33 |  630.2 | 1056.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    37.99 ms |  1.31 |  33 |  876.1 |  759.8 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    82.68 ms |  2.84 |  33 |  402.5 | 1653.6 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   600.70 ms |  1.00 | 665 | 1108.4 |  600.7 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   257.43 ms |  0.43 | 665 | 2586.4 |  257.4 |  268.09 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,041.79 ms |  1.74 | 665 |  639.1 | 1041.8 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,262.24 ms |  2.10 | 665 |  527.5 | 1262.2 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,616.12 ms |  2.69 | 665 |  412.0 | 1616.1 |  260.58 MB |        1.00 |
