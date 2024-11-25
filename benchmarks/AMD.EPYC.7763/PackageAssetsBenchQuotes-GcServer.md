```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-HVPBTZ : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-HVPBTZ  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    40.61 ms |  1.00 |  33 |  819.6 |  812.1 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    20.94 ms |  0.52 |  33 | 1589.1 |  418.9 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    65.44 ms |  1.61 |  33 |  508.6 | 1308.7 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    41.99 ms |  1.03 |  33 |  792.6 |  839.8 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   120.35 ms |  2.96 |  33 |  276.6 | 2406.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   775.60 ms |  1.00 | 665 |  858.4 |  775.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   386.10 ms |  0.50 | 665 | 1724.4 |  386.1 |  263.07 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,276.57 ms |  1.65 | 665 |  521.6 | 1276.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,451.32 ms |  1.87 | 665 |  458.8 | 1451.3 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,331.93 ms |  3.01 | 665 |  285.5 | 2331.9 |  260.58 MB |        1.00 |
