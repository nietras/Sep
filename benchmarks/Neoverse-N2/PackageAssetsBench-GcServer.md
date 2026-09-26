```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 11.36 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Server=True  Toolchain=net11.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.58 ms |  1.00 |  29 |  892.7 |  651.6 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    12.03 ms |  0.37 |  29 | 2417.3 |  240.6 |   13.56 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    52.95 ms |  1.63 |  29 |  549.3 | 1059.0 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    51.33 ms |  1.58 |  29 |  566.7 | 1026.5 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   101.03 ms |  3.10 |  29 |  287.9 | 2020.5 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   647.09 ms |  1.00 | 581 |  899.2 |  647.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   216.23 ms |  0.33 | 581 | 2691.0 |  216.2 |   267.4 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,065.44 ms |  1.65 | 581 |  546.1 | 1065.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,069.53 ms |  1.65 | 581 |  544.0 | 1069.5 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,984.34 ms |  3.07 | 581 |  293.2 | 1984.3 |  260.58 MB |        1.00 |
