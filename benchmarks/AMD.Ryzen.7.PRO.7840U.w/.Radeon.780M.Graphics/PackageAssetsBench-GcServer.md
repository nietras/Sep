```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-KCKHSP : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-NSODLD : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Server=True  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Runtime  | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |--------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | .NET 8.0 | Asset | 50000   |    35.45 ms |  1.00 |  29 |  823.2 |  708.9 |   13.48 MB |        1.00 |
| Sep_MT___ | .NET 8.0 | Asset | 50000   |    12.27 ms |  0.35 |  29 | 2377.3 |  245.5 |   13.57 MB |        1.01 |
| Sylvan___ | .NET 8.0 | Asset | 50000   |    49.67 ms |  1.40 |  29 |  587.5 |  993.4 |   13.63 MB |        1.01 |
| ReadLine_ | .NET 8.0 | Asset | 50000   |    46.88 ms |  1.32 |  29 |  622.4 |  937.7 |   99.74 MB |        7.40 |
| CsvHelper | .NET 8.0 | Asset | 50000   |   120.46 ms |  3.40 |  29 |  242.3 | 2409.1 |   13.64 MB |        1.01 |
| Sep______ | .NET 9.0 | Asset | 50000   |    33.41 ms |  0.94 |  29 |  873.5 |  668.2 |   13.48 MB |        1.00 |
| Sep_MT___ | .NET 9.0 | Asset | 50000   |    12.40 ms |  0.35 |  29 | 2352.8 |  248.1 |   13.57 MB |        1.01 |
| Sylvan___ | .NET 9.0 | Asset | 50000   |    43.36 ms |  1.22 |  29 |  673.1 |  867.1 |   13.63 MB |        1.01 |
| ReadLine_ | .NET 9.0 | Asset | 50000   |    46.90 ms |  1.32 |  29 |  622.2 |  938.0 |   99.74 MB |        7.40 |
| CsvHelper | .NET 9.0 | Asset | 50000   |   118.22 ms |  3.34 |  29 |  246.8 | 2364.5 |   13.65 MB |        1.01 |
|           |          |       |         |             |       |     |        |        |            |             |
| Sep______ | .NET 8.0 | Asset | 1000000 |   745.32 ms |  1.00 | 583 |  783.3 |  745.3 |  260.41 MB |        1.00 |
| Sep_MT___ | .NET 8.0 | Asset | 1000000 |   264.34 ms |  0.36 | 583 | 2208.5 |  264.3 |  261.65 MB |        1.00 |
| Sylvan___ | .NET 8.0 | Asset | 1000000 | 1,032.07 ms |  1.39 | 583 |  565.6 | 1032.1 |  260.57 MB |        1.00 |
| ReadLine_ | .NET 8.0 | Asset | 1000000 | 1,034.99 ms |  1.39 | 583 |  564.0 | 1035.0 | 1991.04 MB |        7.65 |
| CsvHelper | .NET 8.0 | Asset | 1000000 | 2,458.27 ms |  3.30 | 583 |  237.5 | 2458.3 |  260.58 MB |        1.00 |
| Sep______ | .NET 9.0 | Asset | 1000000 |   776.81 ms |  1.04 | 583 |  751.5 |  776.8 |  260.41 MB |        1.00 |
| Sep_MT___ | .NET 9.0 | Asset | 1000000 |   273.09 ms |  0.37 | 583 | 2137.7 |  273.1 |  261.64 MB |        1.00 |
| Sylvan___ | .NET 9.0 | Asset | 1000000 |   926.67 ms |  1.24 | 583 |  630.0 |  926.7 |  260.57 MB |        1.00 |
| ReadLine_ | .NET 9.0 | Asset | 1000000 | 1,040.58 ms |  1.40 | 583 |  561.0 | 1040.6 | 1991.05 MB |        7.65 |
| CsvHelper | .NET 9.0 | Asset | 1000000 | 2,414.30 ms |  3.24 | 583 |  241.8 | 2414.3 |  260.58 MB |        1.00 |
