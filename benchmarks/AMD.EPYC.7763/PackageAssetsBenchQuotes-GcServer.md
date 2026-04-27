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
| Sep______ | Asset | 50000   |    40.50 ms |  1.00 |  33 |  824.2 |  810.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    24.13 ms |  0.60 |  33 | 1383.5 |  482.5 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    61.50 ms |  1.52 |  33 |  542.7 | 1230.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    61.80 ms |  1.53 |  33 |  540.1 | 1236.0 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   119.49 ms |  2.95 |  33 |  279.3 | 2389.8 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   895.41 ms |  1.00 | 667 |  745.7 |  895.4 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   463.72 ms |  0.52 | 667 | 1439.9 |  463.7 |  261.87 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,300.50 ms |  1.46 | 667 |  513.4 | 1300.5 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,387.64 ms |  1.55 | 667 |  481.2 | 1387.6 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,444.88 ms |  2.74 | 667 |  273.1 | 2444.9 |  260.58 MB |        1.00 |
