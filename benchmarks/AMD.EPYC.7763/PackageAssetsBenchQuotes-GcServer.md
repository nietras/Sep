```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-UGLWRK : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-UGLWRK  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    40.47 ms |  1.00 |  33 |  822.4 |  809.4 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    21.06 ms |  0.52 |  33 | 1580.2 |  421.2 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    65.00 ms |  1.61 |  33 |  512.0 | 1300.0 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    42.03 ms |  1.04 |  33 |  791.9 |  840.6 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   120.30 ms |  2.97 |  33 |  276.7 | 2406.0 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   770.29 ms |  1.00 | 665 |  864.3 |  770.3 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   382.03 ms |  0.50 | 665 | 1742.8 |  382.0 |  262.66 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,305.41 ms |  1.69 | 665 |  510.0 | 1305.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,497.48 ms |  1.94 | 665 |  444.6 | 1497.5 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,342.07 ms |  3.04 | 665 |  284.3 | 2342.1 |  260.58 MB |        1.00 |
