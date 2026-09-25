```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.34 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Server=True  Toolchain=net11.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    33.32 ms |  1.00 |  29 |  873.1 |  666.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    17.84 ms |  0.54 |  29 | 1630.8 |  356.7 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    43.53 ms |  1.31 |  29 |  668.2 |  870.6 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    58.52 ms |  1.76 |  29 |  497.0 | 1170.5 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   120.25 ms |  3.62 |  29 |  241.9 | 2404.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   661.51 ms |  1.00 | 581 |  879.6 |  661.5 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   346.61 ms |  0.52 | 581 | 1678.8 |  346.6 |  269.25 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   887.62 ms |  1.34 | 581 |  655.5 |  887.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,265.98 ms |  1.91 | 581 |  459.6 | 1266.0 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,431.03 ms |  3.67 | 581 |  239.4 | 2431.0 |  260.58 MB |        1.00 |
