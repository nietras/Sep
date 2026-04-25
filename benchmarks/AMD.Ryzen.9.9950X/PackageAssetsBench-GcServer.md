```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7184/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    14.258 ms |  1.00 |  29 | 2046.6 |  285.2 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     3.261 ms |  0.23 |  29 | 8947.4 |   65.2 |   13.65 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    18.170 ms |  1.27 |  29 | 1606.0 |  363.4 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    19.166 ms |  1.34 |  29 | 1522.6 |  383.3 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    49.240 ms |  3.45 |  29 |  592.6 |  984.8 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   287.939 ms |  1.00 | 583 | 2027.5 |  287.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |    58.468 ms |  0.20 | 583 | 9984.7 |   58.5 |  261.67 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   370.560 ms |  1.29 | 583 | 1575.4 |  370.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   365.042 ms |  1.27 | 583 | 1599.2 |  365.0 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,001.476 ms |  3.48 | 583 |  582.9 | 1001.5 |  260.58 MB |        1.00 |
