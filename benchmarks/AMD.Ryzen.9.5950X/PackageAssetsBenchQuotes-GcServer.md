```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2
  Job-ANVGUP : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-ANVGUP  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    25.49 ms |  1.00 |  33 | 1309.5 |  509.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.02 ms |  0.43 |  33 | 3027.8 |  220.5 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    42.03 ms |  1.65 |  33 |  794.1 |  840.6 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    38.66 ms |  1.52 |  33 |  863.3 |  773.2 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    78.18 ms |  3.07 |  33 |  426.9 | 1563.7 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   519.82 ms |  1.00 | 667 | 1284.5 |  519.8 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   210.37 ms |  0.40 | 667 | 3173.9 |  210.4 |  261.48 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   870.28 ms |  1.67 | 667 |  767.2 |  870.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   630.49 ms |  1.21 | 667 | 1059.0 |  630.5 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,559.67 ms |  3.00 | 667 |  428.1 | 1559.7 |  260.58 MB |        1.00 |
