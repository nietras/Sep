```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.1 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-CZUNEQ : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-CZUNEQ  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    39.11 ms |  1.00 |  33 |  851.0 |  782.2 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    23.02 ms |  0.59 |  33 | 1445.6 |  460.5 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    64.15 ms |  1.64 |  33 |  518.8 | 1283.0 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    69.84 ms |  1.79 |  33 |  476.5 | 1396.8 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   119.33 ms |  3.05 |  33 |  278.9 | 2386.7 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   851.15 ms |  1.00 | 665 |  782.2 |  851.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   433.22 ms |  0.51 | 665 | 1536.9 |  433.2 |  262.82 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,328.75 ms |  1.56 | 665 |  501.1 | 1328.7 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,477.57 ms |  1.74 | 665 |  450.6 | 1477.6 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,519.43 ms |  2.96 | 665 |  264.3 | 2519.4 |  260.59 MB |        1.00 |
