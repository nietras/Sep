```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.1 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-OIQWSK : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-OIQWSK  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    40.58 ms |  1.00 |  33 |  820.3 |  811.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    22.12 ms |  0.55 |  33 | 1504.7 |  442.4 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    65.08 ms |  1.60 |  33 |  511.4 | 1301.7 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    67.34 ms |  1.66 |  33 |  494.3 | 1346.7 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   118.82 ms |  2.93 |  33 |  280.1 | 2376.4 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   819.13 ms |  1.00 | 665 |  812.8 |  819.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   428.63 ms |  0.52 | 665 | 1553.3 |  428.6 |  263.24 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,324.33 ms |  1.62 | 665 |  502.7 | 1324.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,473.27 ms |  1.80 | 665 |  451.9 | 1473.3 | 2385.08 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,389.26 ms |  2.92 | 665 |  278.7 | 2389.3 |  260.58 MB |        1.00 |
