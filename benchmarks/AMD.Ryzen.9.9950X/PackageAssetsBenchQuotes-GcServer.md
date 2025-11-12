```

BenchmarkDotNet v0.15.6, Windows 10 (10.0.19045.6575/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |  17.957 ms |  1.00 |  33 | 1858.7 |  359.1 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   6.163 ms |  0.34 |  33 | 5415.4 |  123.3 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  27.490 ms |  1.53 |  33 | 1214.2 |  549.8 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |  22.058 ms |  1.23 |  33 | 1513.1 |  441.2 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |  47.823 ms |  2.66 |  33 |  697.9 |  956.5 |   13.64 MB |        1.01 |
|           |       |         |            |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 | 368.631 ms |  1.00 | 667 | 1811.3 |  368.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 100.499 ms |  0.27 | 667 | 6643.9 |  100.5 |  262.23 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 544.577 ms |  1.48 | 667 | 1226.1 |  544.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 406.199 ms |  1.10 | 667 | 1643.8 |  406.2 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 944.811 ms |  2.56 | 667 |  706.7 |  944.8 |  260.58 MB |        1.00 |
