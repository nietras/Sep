```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2
  Job-WYIEQQ : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2

Job=Job-WYIEQQ  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    20.951 ms |  1.00 |  29 | 1392.9 |  419.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     6.614 ms |  0.32 |  29 | 4411.8 |  132.3 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    27.761 ms |  1.33 |  29 | 1051.2 |  555.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    33.516 ms |  1.60 |  29 |  870.7 |  670.3 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    77.007 ms |  3.68 |  29 |  378.9 | 1540.1 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   432.887 ms |  1.00 | 583 | 1348.6 |  432.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   119.430 ms |  0.28 | 583 | 4888.1 |  119.4 |  261.39 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   559.550 ms |  1.29 | 583 | 1043.3 |  559.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   573.637 ms |  1.33 | 583 | 1017.7 |  573.6 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,537.602 ms |  3.55 | 583 |  379.7 | 1537.6 |  260.58 MB |        1.00 |
