```

BenchmarkDotNet v0.13.11, Ubuntu 22.04.2 LTS (Jammy Jellyfish)
Unknown processor
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  Job-HCDRZS : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD

Job=Job-HCDRZS  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350.0000 ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | RatioSD | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|--------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    56.29 ms |  1.00 |    0.00 |  29 |  516.7 | 1125.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    20.66 ms |  0.37 |    0.02 |  29 | 1407.7 |  413.2 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    88.70 ms |  1.58 |    0.08 |  29 |  327.9 | 1773.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    67.01 ms |  1.19 |    0.04 |  29 |  434.1 | 1340.1 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   158.71 ms |  2.83 |    0.10 |  29 |  183.3 | 3174.2 |   13.64 MB |        1.01 |
|           |       |         |             |       |         |     |        |        |            |             |
| Sep______ | Asset | 1000000 | 1,110.22 ms |  1.00 |    0.00 | 581 |  524.1 | 1110.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   402.71 ms |  0.36 |    0.00 | 581 | 1444.9 |  402.7 |  265.42 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,759.98 ms |  1.58 |    0.01 | 581 |  330.6 | 1760.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,575.08 ms |  1.43 |    0.07 | 581 |  369.4 | 1575.1 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 3,167.05 ms |  2.85 |    0.01 | 581 |  183.7 | 3167.1 |  260.58 MB |        1.00 |
