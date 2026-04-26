```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.86GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    38.03 ms |  1.00 |  33 |  875.1 |  760.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    22.26 ms |  0.59 |  33 | 1494.9 |  445.3 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    61.00 ms |  1.60 |  33 |  545.6 | 1220.0 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    67.50 ms |  1.77 |  33 |  493.1 | 1349.9 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   114.65 ms |  3.01 |  33 |  290.3 | 2293.1 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   785.52 ms |  1.00 | 665 |  847.6 |  785.5 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   449.62 ms |  0.57 | 665 | 1480.8 |  449.6 |  261.88 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,240.42 ms |  1.58 | 665 |  536.8 | 1240.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,400.28 ms |  1.78 | 665 |  475.5 | 1400.3 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,350.30 ms |  2.99 | 665 |  283.3 | 2350.3 |  260.58 MB |        1.00 |
