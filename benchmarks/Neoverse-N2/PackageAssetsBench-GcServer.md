```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    34.50 ms |  1.00 |  29 |  843.0 |  690.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.73 ms |  0.34 |  29 | 2479.5 |  234.6 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    55.52 ms |  1.61 |  29 |  523.9 | 1110.4 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    53.67 ms |  1.56 |  29 |  541.9 | 1073.5 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   100.93 ms |  2.93 |  29 |  288.2 | 2018.6 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   662.83 ms |  1.00 | 581 |  877.9 |  662.8 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   210.24 ms |  0.32 | 581 | 2767.6 |  210.2 |  267.97 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,103.25 ms |  1.66 | 581 |  527.4 | 1103.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,078.47 ms |  1.63 | 581 |  539.5 | 1078.5 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,994.21 ms |  3.01 | 581 |  291.8 | 1994.2 |  260.58 MB |        1.00 |
