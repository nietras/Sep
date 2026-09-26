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
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    26.68 ms |  1.00 |  33 | 1247.4 |  533.6 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.52 ms |  0.43 |  33 | 2888.4 |  230.4 |    13.6 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    48.46 ms |  1.82 |  33 |  686.7 |  969.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    46.65 ms |  1.75 |  33 |  713.5 |  932.9 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    78.55 ms |  2.94 |  33 |  423.7 | 1571.1 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   547.35 ms |  1.00 | 665 | 1216.4 |  547.4 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   236.36 ms |  0.43 | 665 | 2816.9 |  236.4 |  268.62 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   948.26 ms |  1.73 | 665 |  702.1 |  948.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   948.32 ms |  1.73 | 665 |  702.1 |  948.3 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,562.30 ms |  2.86 | 665 |  426.2 | 1562.3 |  260.58 MB |        1.00 |
