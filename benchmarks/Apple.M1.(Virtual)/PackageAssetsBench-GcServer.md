```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    21.52 ms |  1.00 |  29 | 1351.6 |  430.4 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    10.58 ms |  0.49 |  29 | 2747.9 |  211.7 |   13.62 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    45.61 ms |  2.12 |  29 |  637.7 |  912.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    29.76 ms |  1.39 |  29 |  977.3 |  595.2 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    70.79 ms |  3.30 |  29 |  410.9 | 1415.8 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   464.13 ms |  1.00 | 581 | 1253.7 |  464.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   207.97 ms |  0.45 | 581 | 2797.8 |  208.0 |   269.7 MB |        1.04 |
| Sylvan___ | Asset | 1000000 | 1,016.06 ms |  2.19 | 581 |  572.7 | 1016.1 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,029.08 ms |  2.22 | 581 |  565.4 | 1029.1 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,634.85 ms |  3.52 | 581 |  355.9 | 1634.8 |  260.58 MB |        1.00 |
