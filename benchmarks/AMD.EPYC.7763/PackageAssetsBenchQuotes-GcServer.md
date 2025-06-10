```

BenchmarkDotNet v0.15.1, Linux Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-GIXJJC : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-GIXJJC  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    39.96 ms |  1.00 |  33 |  833.0 |  799.1 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    22.53 ms |  0.56 |  33 | 1477.5 |  450.5 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    64.04 ms |  1.60 |  33 |  519.7 | 1280.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    66.74 ms |  1.67 |  33 |  498.7 | 1334.8 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   119.09 ms |  2.98 |  33 |  279.5 | 2381.8 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   823.91 ms |  1.00 | 665 |  808.1 |  823.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   435.57 ms |  0.53 | 665 | 1528.6 |  435.6 |  263.42 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,300.41 ms |  1.58 | 665 |  512.0 | 1300.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,409.54 ms |  1.71 | 665 |  472.4 | 1409.5 | 2385.08 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,398.79 ms |  2.91 | 665 |  277.6 | 2398.8 |  260.58 MB |        1.00 |
