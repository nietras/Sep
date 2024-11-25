```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-HVPBTZ : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-HVPBTZ  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.39 ms |  1.00 |  29 |  898.1 |  647.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    14.89 ms |  0.46 |  29 | 1953.6 |  297.8 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    41.94 ms |  1.30 |  29 |  693.5 |  838.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    37.66 ms |  1.16 |  29 |  772.3 |  753.3 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   121.64 ms |  3.76 |  29 |  239.1 | 2432.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   620.61 ms |  1.00 | 581 |  937.6 |  620.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   287.75 ms |  0.46 | 581 | 2022.2 |  287.7 |  269.46 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   828.44 ms |  1.33 | 581 |  702.4 |  828.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,307.41 ms |  2.11 | 581 |  445.1 | 1307.4 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,355.53 ms |  3.80 | 581 |  247.0 | 2355.5 |  260.58 MB |        1.00 |
