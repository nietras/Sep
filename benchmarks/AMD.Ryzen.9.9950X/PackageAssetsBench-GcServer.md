```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 9950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-OKSCZA : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-OKSCZA  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    14.536 ms |  1.00 |  29 | 2007.5 |  290.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     3.606 ms |  0.25 |  29 | 8091.8 |   72.1 |   13.65 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    20.457 ms |  1.41 |  29 | 1426.5 |  409.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    20.307 ms |  1.40 |  29 | 1437.0 |  406.1 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    54.022 ms |  3.72 |  29 |  540.2 | 1080.4 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   291.979 ms |  1.00 | 583 | 1999.4 |  292.0 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |    72.213 ms |  0.25 | 583 | 8084.1 |   72.2 |  261.63 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   413.265 ms |  1.42 | 583 | 1412.6 |  413.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   377.033 ms |  1.29 | 583 | 1548.4 |  377.0 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,005.323 ms |  3.44 | 583 |  580.7 | 1005.3 |  260.58 MB |        1.00 |
