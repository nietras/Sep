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
| Sep______ | Asset | 50000   |    25.46 ms |  1.00 |  33 | 1307.4 |  509.2 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    13.12 ms |  0.52 |  33 | 2536.6 |  262.4 |   13.66 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    45.99 ms |  1.81 |  33 |  723.7 |  919.7 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    37.51 ms |  1.47 |  33 |  887.3 |  750.2 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    82.55 ms |  3.24 |  33 |  403.2 | 1650.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   553.56 ms |  1.00 | 665 | 1202.8 |  553.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   228.07 ms |  0.41 | 665 | 2919.3 |  228.1 |  270.92 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   941.36 ms |  1.70 | 665 |  707.3 |  941.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,056.83 ms |  1.91 | 665 |  630.0 | 1056.8 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,634.21 ms |  2.95 | 665 |  407.4 | 1634.2 |  260.58 MB |        1.00 |
