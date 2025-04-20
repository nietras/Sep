```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-YRWPAD : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-YRWPAD  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    39.91 ms |  1.00 |  33 |  836.4 |  798.2 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    18.80 ms |  0.47 |  33 | 1775.4 |  376.0 |   13.58 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    67.17 ms |  1.68 |  33 |  496.9 | 1343.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    67.19 ms |  1.68 |  33 |  496.8 | 1343.7 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   113.44 ms |  2.84 |  33 |  294.2 | 2268.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   871.42 ms |  1.00 | 667 |  766.2 |  871.4 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   343.11 ms |  0.39 | 667 | 1946.0 |  343.1 |   261.3 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,358.88 ms |  1.56 | 667 |  491.4 | 1358.9 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,157.77 ms |  1.33 | 667 |  576.7 | 1157.8 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,299.56 ms |  2.64 | 667 |  290.4 | 2299.6 |  260.58 MB |        1.00 |
