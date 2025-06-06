```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-BLMCOC : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-BLMCOC  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    39.18 ms |  1.00 |  33 |  849.4 |  783.6 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    22.12 ms |  0.56 |  33 | 1504.4 |  442.5 |   13.55 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    63.41 ms |  1.62 |  33 |  524.9 | 1268.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    66.94 ms |  1.71 |  33 |  497.2 | 1338.8 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   119.66 ms |  3.05 |  33 |  278.1 | 2393.2 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   843.12 ms |  1.00 | 665 |  789.7 |  843.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   431.03 ms |  0.51 | 665 | 1544.7 |  431.0 |  262.76 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,285.46 ms |  1.52 | 665 |  517.9 | 1285.5 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,401.26 ms |  1.66 | 665 |  475.1 | 1401.3 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,392.21 ms |  2.84 | 665 |  278.3 | 2392.2 |  260.59 MB |        1.00 |
