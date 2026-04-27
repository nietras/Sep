```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 3.24GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______ | Asset | 50000   |    30.67 ms |  1.00 |  29 |  948.5 |  613.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    17.07 ms |  0.56 |  29 | 1704.0 |  341.4 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    41.63 ms |  1.36 |  29 |  698.7 |  832.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    58.75 ms |  1.92 |  29 |  495.0 | 1175.1 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   114.31 ms |  3.73 |  29 |  254.5 | 2286.1 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   640.63 ms |  1.00 | 581 |  908.3 |  640.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   348.07 ms |  0.54 | 581 | 1671.7 |  348.1 |  269.03 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   811.52 ms |  1.27 | 581 |  717.0 |  811.5 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,205.96 ms |  1.88 | 581 |  482.5 | 1206.0 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,315.88 ms |  3.62 | 581 |  251.3 | 2315.9 |  260.58 MB |        1.00 |
