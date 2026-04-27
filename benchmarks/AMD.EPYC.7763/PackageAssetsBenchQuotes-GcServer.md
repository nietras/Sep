```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 3.24GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______ | Asset | 50000   |    39.29 ms |  1.00 |  33 |  847.0 |  785.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    21.90 ms |  0.56 |  33 | 1519.4 |  438.1 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    60.59 ms |  1.54 |  33 |  549.3 | 1211.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    64.77 ms |  1.65 |  33 |  513.8 | 1295.5 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   118.71 ms |  3.02 |  33 |  280.4 | 2374.1 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   765.86 ms |  1.00 | 665 |  869.3 |  765.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   422.00 ms |  0.55 | 665 | 1577.7 |  422.0 |   263.2 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,217.00 ms |  1.59 | 665 |  547.1 | 1217.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,344.49 ms |  1.76 | 665 |  495.2 | 1344.5 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,365.39 ms |  3.09 | 665 |  281.5 | 2365.4 |  260.58 MB |        1.00 |
