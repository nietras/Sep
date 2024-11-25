```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-WTIUBR : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-WTIUBR  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.87 ms |  1.00 |  29 |  887.7 |  657.4 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    12.08 ms |  0.37 |  29 | 2415.5 |  241.6 |   13.57 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    44.10 ms |  1.34 |  29 |  661.7 |  881.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    46.91 ms |  1.43 |  29 |  622.1 |  938.2 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   117.89 ms |  3.59 |  29 |  247.5 | 2357.8 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   712.78 ms |  1.00 | 583 |  819.0 |  712.8 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   280.21 ms |  0.39 | 583 | 2083.3 |  280.2 |   261.5 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   911.34 ms |  1.28 | 583 |  640.6 |  911.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,080.48 ms |  1.52 | 583 |  540.3 | 1080.5 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,393.86 ms |  3.37 | 583 |  243.9 | 2393.9 |  260.58 MB |        1.00 |
