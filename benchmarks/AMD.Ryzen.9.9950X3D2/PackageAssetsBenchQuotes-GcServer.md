```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 9950X3D2 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |  17.980 ms |  1.00 |  33 | 1856.4 |  359.6 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   6.572 ms |  0.37 |  33 | 5078.3 |  131.4 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  26.947 ms |  1.50 |  33 | 1238.6 |  538.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |  17.869 ms |  0.99 |  33 | 1867.9 |  357.4 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |  47.766 ms |  2.66 |  33 |  698.8 |  955.3 |   13.64 MB |        1.01 |
|           |       |         |            |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 | 375.499 ms |  1.00 | 667 | 1778.2 |  375.5 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 111.924 ms |  0.30 | 667 | 5965.7 |  111.9 |  262.04 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 563.109 ms |  1.50 | 667 | 1185.7 |  563.1 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 514.825 ms |  1.37 | 667 | 1297.0 |  514.8 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 963.436 ms |  2.57 | 667 |  693.0 |  963.4 |  260.58 MB |        1.00 |
