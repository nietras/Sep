```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-XBPEID : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.119 ms |  1.00 | 20 | 6514.3 |  124.8 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.690 ms |  1.18 | 20 | 5506.9 |  147.6 |    10.71 KB |        8.53 |
| ReadLine_ | Row    | 25000 |  16.859 ms |  5.40 | 20 | 1205.3 |  674.4 | 73489.66 KB |   58,562.97 |
| CsvHelper | Row    | 25000 |  39.719 ms | 12.73 | 20 |  511.6 | 1588.8 |    20.03 KB |       15.96 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.215 ms |  1.00 | 20 | 4821.0 |  168.6 |     1.26 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.074 ms |  1.44 | 20 | 3345.4 |  243.0 |    10.71 KB |        8.52 |
| ReadLine_ | Cols   | 25000 |  17.072 ms |  4.05 | 20 | 1190.3 |  682.9 | 73489.65 KB |   58,471.95 |
| CsvHelper | Cols   | 25000 |  42.474 ms | 10.08 | 20 |  478.4 | 1699.0 | 21340.26 KB |   16,979.35 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  32.362 ms |  1.00 | 20 |  627.9 | 1294.5 |     8.32 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   5.940 ms |  0.18 | 20 | 3420.6 |  237.6 |   114.06 KB |       13.71 |
| Sylvan___ | Floats | 25000 |  81.574 ms |  2.52 | 20 |  249.1 | 3263.0 |    18.88 KB |        2.27 |
| ReadLine_ | Floats | 25000 | 106.297 ms |  3.28 | 20 |  191.2 | 4251.9 | 73493.12 KB |    8,837.12 |
| CsvHelper | Floats | 25000 | 156.658 ms |  4.84 | 20 |  129.7 | 6266.3 | 22062.72 KB |    2,652.91 |
