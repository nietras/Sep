```

BenchmarkDotNet v0.16.0-preview.1, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
Memory: 7 GB Total, 0.09 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Server=True  Toolchain=net11.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    22.873 ms |  1.00 |  29 | 1271.6 |  457.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     9.462 ms |  0.41 |  29 | 3074.0 |  189.2 |   13.59 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    44.864 ms |  1.96 |  29 |  648.3 |  897.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    37.137 ms |  1.62 |  29 |  783.2 |  742.7 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    69.252 ms |  3.03 |  29 |  420.0 | 1385.0 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   466.013 ms |  1.00 | 581 | 1248.6 |  466.0 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   196.468 ms |  0.42 | 581 | 2961.7 |  196.5 |  270.79 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   988.247 ms |  2.12 | 581 |  588.8 |  988.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   948.823 ms |  2.04 | 581 |  613.3 |  948.8 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,541.418 ms |  3.31 | 581 |  377.5 | 1541.4 |  260.58 MB |        1.00 |
