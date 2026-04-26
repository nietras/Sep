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
| Sep______ | Asset | 50000   |    30.01 ms |  1.01 |  33 | 1108.9 |  600.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    18.51 ms |  0.62 |  33 | 1797.6 |  370.3 |   13.61 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    58.31 ms |  1.96 |  33 |  570.8 | 1166.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    41.55 ms |  1.40 |  33 |  801.1 |  830.9 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    98.15 ms |  3.31 |  33 |  339.1 | 1963.0 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   650.08 ms |  1.00 | 665 | 1024.2 |  650.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   276.54 ms |  0.43 | 665 | 2407.6 |  276.5 |  266.96 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,208.29 ms |  1.86 | 665 |  551.0 | 1208.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 3,161.23 ms |  4.87 | 665 |  210.6 | 3161.2 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,399.26 ms |  3.70 | 665 |  277.5 | 2399.3 |  260.58 MB |        1.00 |
