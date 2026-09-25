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
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    40.24 ms |  1.00 |  33 |  827.1 |  804.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    22.85 ms |  0.57 |  33 | 1456.3 |  457.1 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    61.96 ms |  1.54 |  33 |  537.1 | 1239.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    60.34 ms |  1.50 |  33 |  551.6 | 1206.8 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   114.46 ms |  2.85 |  33 |  290.8 | 2289.2 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   841.30 ms |  1.00 | 665 |  791.4 |  841.3 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   439.60 ms |  0.52 | 665 | 1514.6 |  439.6 |  262.59 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,262.73 ms |  1.50 | 665 |  527.3 | 1262.7 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,427.46 ms |  1.70 | 665 |  466.4 | 1427.5 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,336.71 ms |  2.78 | 665 |  284.9 | 2336.7 |  260.58 MB |        1.00 |
