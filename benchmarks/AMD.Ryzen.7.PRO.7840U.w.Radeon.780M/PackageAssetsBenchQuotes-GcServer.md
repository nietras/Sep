```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-WTIUBR : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-WTIUBR  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    39.42 ms |  1.00 |  33 |  846.8 |  788.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    23.61 ms |  0.60 |  33 | 1413.5 |  472.3 |   13.59 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    65.68 ms |  1.67 |  33 |  508.2 | 1313.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    83.86 ms |  2.13 |  33 |  398.0 | 1677.2 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   112.64 ms |  2.86 |  33 |  296.3 | 2252.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   853.65 ms |  1.00 | 667 |  782.2 |  853.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   383.82 ms |  0.45 | 667 | 1739.6 |  383.8 |  261.86 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,351.84 ms |  1.58 | 667 |  493.9 | 1351.8 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,173.87 ms |  1.38 | 667 |  568.8 | 1173.9 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,305.37 ms |  2.70 | 667 |  289.6 | 2305.4 |  260.58 MB |        1.00 |
