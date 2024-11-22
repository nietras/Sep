```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  Job-ZCGTWP : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2

Job=Job-ZCGTWP  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    26.99 ms |  1.00 |  33 | 1236.4 |  539.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    10.73 ms |  0.40 |  33 | 3110.0 |  214.6 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    45.23 ms |  1.68 |  33 |  738.0 |  904.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    40.03 ms |  1.48 |  33 |  833.8 |  800.6 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    92.14 ms |  3.41 |  33 |  362.2 | 1842.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   545.56 ms |  1.00 | 667 | 1223.9 |  545.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   211.38 ms |  0.39 | 667 | 3158.8 |  211.4 |  261.26 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   918.14 ms |  1.68 | 667 |  727.2 |  918.1 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   663.73 ms |  1.22 | 667 | 1006.0 |  663.7 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,831.01 ms |  3.36 | 667 |  364.7 | 1831.0 |  260.58 MB |        1.00 |
