```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32522/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______ | Asset | 50000   |    45.34 ms |  1.01 |  33 |  736.2 |  906.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    24.94 ms |  0.55 |  33 | 1338.3 |  498.8 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    70.33 ms |  1.56 |  33 |  474.6 | 1406.7 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    63.15 ms |  1.40 |  33 |  528.6 | 1262.9 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   139.14 ms |  3.09 |  33 |  239.9 | 2782.7 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   850.86 ms |  1.00 | 667 |  784.7 |  850.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   431.51 ms |  0.51 | 667 | 1547.4 |  431.5 |  264.59 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,260.38 ms |  1.48 | 667 |  529.8 | 1260.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,298.75 ms |  1.53 | 667 |  514.1 | 1298.7 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,466.61 ms |  2.90 | 667 |  270.7 | 2466.6 |  260.58 MB |        1.00 |
