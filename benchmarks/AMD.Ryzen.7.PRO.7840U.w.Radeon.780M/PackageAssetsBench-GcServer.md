```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4602/23H2/2023Update/SunValley3)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-RCSMQK : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-RCSMQK  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.88 ms |  1.00 |  29 |  887.6 |  657.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    12.08 ms |  0.37 |  29 | 2414.9 |  241.7 |   13.57 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    43.15 ms |  1.31 |  29 |  676.3 |  862.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    65.30 ms |  1.99 |  29 |  446.9 | 1305.9 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   117.54 ms |  3.58 |  29 |  248.3 | 2350.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   712.70 ms |  1.00 | 583 |  819.1 |  712.7 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   279.00 ms |  0.39 | 583 | 2092.4 |  279.0 |  262.79 MB |        1.01 |
| Sylvan___ | Asset | 1000000 |   920.38 ms |  1.29 | 583 |  634.3 |  920.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,078.15 ms |  1.52 | 583 |  541.5 | 1078.2 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,417.96 ms |  3.40 | 583 |  241.4 | 2418.0 |  260.58 MB |        1.00 |
