```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26100.6899/24H2/2024Update/HudsonValley)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics 3.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    35.630 ms |  1.00 |  29 |  819.0 |  712.6 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     8.892 ms |  0.25 |  29 | 3281.6 |  177.8 |   13.57 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    38.901 ms |  1.09 |  29 |  750.1 |  778.0 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    43.945 ms |  1.23 |  29 |  664.0 |  878.9 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   116.596 ms |  3.27 |  29 |  250.3 | 2331.9 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   654.638 ms |  1.00 | 583 |  891.8 |  654.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   244.949 ms |  0.37 | 583 | 2383.3 |  244.9 |  262.32 MB |        1.01 |
| Sylvan___ | Asset | 1000000 |   825.727 ms |  1.26 | 583 |  707.0 |  825.7 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   968.707 ms |  1.48 | 583 |  602.6 |  968.7 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,364.922 ms |  3.62 | 583 |  246.9 | 2364.9 |  260.58 MB |        1.00 |
