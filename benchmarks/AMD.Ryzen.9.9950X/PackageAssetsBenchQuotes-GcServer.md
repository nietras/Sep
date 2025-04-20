```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 9950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-OKSCZA : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-OKSCZA  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |  17.916 ms |  1.00 |  33 | 1862.9 |  358.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   6.753 ms |  0.38 |  33 | 4942.7 |  135.1 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  30.678 ms |  1.71 |  33 | 1088.0 |  613.6 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |  22.998 ms |  1.28 |  33 | 1451.3 |  460.0 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |  47.582 ms |  2.66 |  33 |  701.5 |  951.6 |   13.64 MB |        1.01 |
|           |       |         |            |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 | 360.848 ms |  1.00 | 667 | 1850.4 |  360.8 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 143.101 ms |  0.40 | 667 | 4666.0 |  143.1 |  261.69 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 624.089 ms |  1.73 | 667 | 1069.9 |  624.1 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 419.179 ms |  1.16 | 667 | 1592.9 |  419.2 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 953.606 ms |  2.64 | 667 |  700.2 |  953.6 |  260.58 MB |        1.00 |
