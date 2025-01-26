```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4602/23H2/2023Update/SunValley3)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-RCSMQK : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-RCSMQK  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    40.64 ms |  1.00 |  33 |  821.3 |  812.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    22.02 ms |  0.54 |  33 | 1515.5 |  440.5 |   13.59 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    65.40 ms |  1.61 |  33 |  510.3 | 1308.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    86.96 ms |  2.14 |  33 |  383.8 | 1739.1 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   113.09 ms |  2.78 |  33 |  295.1 | 2261.8 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   837.08 ms |  1.00 | 667 |  797.7 |  837.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   424.07 ms |  0.51 | 667 | 1574.5 |  424.1 |  262.92 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,355.96 ms |  1.62 | 667 |  492.4 | 1356.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,145.13 ms |  1.37 | 667 |  583.1 | 1145.1 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,308.37 ms |  2.76 | 667 |  289.3 | 2308.4 |  260.58 MB |        1.00 |
