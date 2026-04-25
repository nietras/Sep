```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.86GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    31.25 ms |  1.00 |  29 |  930.7 |  625.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    17.77 ms |  0.57 |  29 | 1636.7 |  355.4 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    43.06 ms |  1.38 |  29 |  675.4 |  861.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    60.18 ms |  1.93 |  29 |  483.3 | 1203.6 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   119.63 ms |  3.83 |  29 |  243.1 | 2392.5 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   642.67 ms |  1.00 | 581 |  905.4 |  642.7 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   362.02 ms |  0.56 | 581 | 1607.3 |  362.0 |  268.73 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   860.37 ms |  1.34 | 581 |  676.3 |  860.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,220.98 ms |  1.90 | 581 |  476.6 | 1221.0 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,421.43 ms |  3.77 | 581 |  240.3 | 2421.4 |  260.58 MB |        1.00 |
