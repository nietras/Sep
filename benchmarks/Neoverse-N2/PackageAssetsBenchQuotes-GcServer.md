```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 11.44 GB Available
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
| Sep______ | Asset | 50000   |    37.84 ms |  1.00 |  33 |  879.6 |  756.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.45 ms |  0.41 |  33 | 2154.1 |  309.0 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    56.72 ms |  1.50 |  33 |  586.8 | 1134.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    60.39 ms |  1.60 |  33 |  551.1 | 1207.7 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   106.88 ms |  2.82 |  33 |  311.4 | 2137.5 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   748.89 ms |  1.00 | 665 |  889.1 |  748.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   246.78 ms |  0.33 | 665 | 2697.9 |  246.8 |  262.68 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,111.32 ms |  1.48 | 665 |  599.1 | 1111.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,265.39 ms |  1.69 | 665 |  526.2 | 1265.4 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,119.59 ms |  2.83 | 665 |  314.1 | 2119.6 |  260.58 MB |        1.00 |
