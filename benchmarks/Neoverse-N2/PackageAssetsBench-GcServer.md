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
| Sep______ | Asset | 50000   |    34.60 ms |  1.00 |  29 |  840.7 |  691.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    12.09 ms |  0.35 |  29 | 2406.6 |  241.7 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    55.35 ms |  1.60 |  29 |  525.5 | 1106.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    55.50 ms |  1.60 |  29 |  524.0 | 1110.1 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   101.37 ms |  2.93 |  29 |  286.9 | 2027.4 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   664.99 ms |  1.00 | 581 |  875.0 |  665.0 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   213.07 ms |  0.32 | 581 | 2730.9 |  213.1 |  266.78 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,108.21 ms |  1.67 | 581 |  525.1 | 1108.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,102.67 ms |  1.66 | 581 |  527.7 | 1102.7 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,100.67 ms |  3.16 | 581 |  277.0 | 2100.7 |  260.58 MB |        1.00 |
