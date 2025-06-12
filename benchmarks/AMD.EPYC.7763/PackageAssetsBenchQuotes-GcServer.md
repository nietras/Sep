```

BenchmarkDotNet v0.15.1, Linux Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-ZOWZUS : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-ZOWZUS  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    39.92 ms |  1.00 |  33 |  833.6 |  798.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    22.60 ms |  0.57 |  33 | 1472.9 |  451.9 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    63.33 ms |  1.59 |  33 |  525.5 | 1266.6 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    67.28 ms |  1.69 |  33 |  494.7 | 1345.5 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   118.11 ms |  2.96 |  33 |  281.8 | 2362.2 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   819.11 ms |  1.00 | 665 |  812.8 |  819.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   428.25 ms |  0.52 | 665 | 1554.7 |  428.2 |  263.02 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,281.90 ms |  1.57 | 665 |  519.4 | 1281.9 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,373.18 ms |  1.68 | 665 |  484.9 | 1373.2 | 2385.08 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,404.07 ms |  2.94 | 665 |  276.9 | 2404.1 |  260.58 MB |        1.00 |
