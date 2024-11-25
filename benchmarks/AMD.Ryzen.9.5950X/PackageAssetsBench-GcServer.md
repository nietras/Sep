```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2
  Job-ANVGUP : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-ANVGUP  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    20.739 ms |  1.00 |  29 | 1407.1 |  414.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     6.042 ms |  0.29 |  29 | 4829.6 |  120.8 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    27.858 ms |  1.34 |  29 | 1047.5 |  557.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    32.844 ms |  1.58 |  29 |  888.5 |  656.9 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    76.303 ms |  3.68 |  29 |  382.4 | 1526.1 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   435.624 ms |  1.00 | 583 | 1340.1 |  435.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   116.180 ms |  0.27 | 583 | 5024.8 |  116.2 |   261.3 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   558.898 ms |  1.28 | 583 | 1044.5 |  558.9 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   573.815 ms |  1.32 | 583 | 1017.4 |  573.8 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,504.212 ms |  3.45 | 583 |  388.1 | 1504.2 |  260.58 MB |        1.00 |
