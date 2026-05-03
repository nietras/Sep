```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32522/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    40.72 ms |  1.00 |  33 |  819.7 |  814.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    22.93 ms |  0.56 |  33 | 1455.5 |  458.6 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    62.88 ms |  1.54 |  33 |  530.8 | 1257.6 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    60.31 ms |  1.48 |  33 |  553.5 | 1206.1 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   118.14 ms |  2.90 |  33 |  282.5 | 2362.8 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   873.73 ms |  1.00 | 667 |  764.2 |  873.7 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   512.99 ms |  0.59 | 667 | 1301.6 |  513.0 |  260.93 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,389.21 ms |  1.59 | 667 |  480.6 | 1389.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,349.82 ms |  1.55 | 667 |  494.7 | 1349.8 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,386.77 ms |  2.73 | 667 |  279.8 | 2386.8 |  260.58 MB |        1.00 |
