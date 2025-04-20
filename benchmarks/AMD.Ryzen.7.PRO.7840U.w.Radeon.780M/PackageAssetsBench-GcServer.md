```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-YRWPAD : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-YRWPAD  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.427 ms |  1.00 |  29 |  899.9 |  648.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     9.377 ms |  0.29 |  29 | 3112.1 |  187.5 |   13.57 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    43.724 ms |  1.35 |  29 |  667.4 |  874.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    48.915 ms |  1.51 |  29 |  596.6 |  978.3 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   118.160 ms |  3.64 |  29 |  247.0 | 2363.2 |   13.65 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   680.684 ms |  1.00 | 583 |  857.6 |  680.7 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   264.669 ms |  0.39 | 583 | 2205.7 |  264.7 |  262.35 MB |        1.01 |
| Sylvan___ | Asset | 1000000 |   910.222 ms |  1.34 | 583 |  641.4 |  910.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,043.395 ms |  1.54 | 583 |  559.5 | 1043.4 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,433.801 ms |  3.58 | 583 |  239.9 | 2433.8 |  260.58 MB |        1.00 |
