```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 9950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.103
  [Host]     : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-USBMCK : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-USBMCK  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    14.863 ms |  1.00 |  29 | 1963.4 |  297.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     4.040 ms |  0.27 |  29 | 7223.1 |   80.8 |   13.65 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    20.807 ms |  1.40 |  29 | 1402.5 |  416.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    20.077 ms |  1.35 |  29 | 1453.4 |  401.5 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    50.566 ms |  3.40 |  29 |  577.1 | 1011.3 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   296.267 ms |  1.00 | 583 | 1970.5 |  296.3 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |    77.297 ms |  0.26 | 583 | 7552.5 |   77.3 |  261.77 MB |        1.01 |
| Sylvan___ | Asset | 1000000 |   425.044 ms |  1.43 | 583 | 1373.5 |  425.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   379.163 ms |  1.28 | 583 | 1539.7 |  379.2 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,013.489 ms |  3.42 | 583 |  576.0 | 1013.5 |  260.58 MB |        1.00 |
