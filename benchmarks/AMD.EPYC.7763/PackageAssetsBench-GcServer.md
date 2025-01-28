```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.1 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-CZUNEQ : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-CZUNEQ  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    33.15 ms |  1.00 |  29 |  877.3 |  663.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    16.87 ms |  0.51 |  29 | 1724.3 |  337.4 |   13.55 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    45.40 ms |  1.37 |  29 |  640.6 |  908.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    62.51 ms |  1.89 |  29 |  465.3 | 1250.2 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   120.39 ms |  3.63 |  29 |  241.6 | 2407.7 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   680.86 ms |  1.00 | 581 |  854.6 |  680.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   346.45 ms |  0.51 | 581 | 1679.5 |  346.5 |  268.56 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   888.66 ms |  1.31 | 581 |  654.8 |  888.7 |  260.58 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,267.46 ms |  1.86 | 581 |  459.1 | 1267.5 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,478.49 ms |  3.64 | 581 |  234.8 | 2478.5 |  260.58 MB |        1.00 |
