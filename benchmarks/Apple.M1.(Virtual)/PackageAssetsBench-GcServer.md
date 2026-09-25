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
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    38.90 ms |  1.00 |  29 |  747.7 |  778.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    33.69 ms |  0.87 |  29 |  863.4 |  673.7 |   13.61 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    73.52 ms |  1.90 |  29 |  395.6 | 1470.4 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    68.04 ms |  1.76 |  29 |  427.5 | 1360.7 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   121.46 ms |  3.14 |  29 |  239.5 | 2429.3 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   788.13 ms |  1.00 | 581 |  738.3 |  788.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   314.03 ms |  0.40 | 581 | 1852.9 |  314.0 |  271.16 MB |        1.04 |
| Sylvan___ | Asset | 1000000 | 1,541.14 ms |  1.97 | 581 |  377.6 | 1541.1 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,230.05 ms |  1.57 | 581 |  473.1 | 1230.1 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,246.40 ms |  2.86 | 581 |  259.0 | 2246.4 |  260.58 MB |        1.00 |
