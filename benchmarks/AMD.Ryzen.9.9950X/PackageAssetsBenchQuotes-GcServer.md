```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 9950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.103
  [Host]     : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-USBMCK : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-USBMCK  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |  17.690 ms |  1.00 |  33 | 1886.8 |  353.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   7.087 ms |  0.40 |  33 | 4709.4 |  141.7 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  30.134 ms |  1.70 |  33 | 1107.6 |  602.7 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |  22.707 ms |  1.28 |  33 | 1469.9 |  454.1 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |  47.463 ms |  2.68 |  33 |  703.2 |  949.3 |   13.64 MB |        1.01 |
|           |       |         |            |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 | 356.488 ms |  1.00 | 667 | 1873.0 |  356.5 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 143.158 ms |  0.40 | 667 | 4664.1 |  143.2 |  262.05 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 606.555 ms |  1.70 | 667 | 1100.8 |  606.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 417.602 ms |  1.17 | 667 | 1598.9 |  417.6 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 953.716 ms |  2.68 | 667 |  700.1 |  953.7 |  260.58 MB |        1.00 |
