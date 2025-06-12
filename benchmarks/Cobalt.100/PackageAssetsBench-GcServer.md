```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-CWGIKZ : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-CWGIKZ  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    34.83 ms |  1.00 |  29 |  837.8 |  696.6 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.85 ms |  0.34 |  29 | 2462.6 |  237.0 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    58.84 ms |  1.69 |  29 |  496.0 | 1176.7 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    46.17 ms |  1.33 |  29 |  632.0 |  923.5 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   103.20 ms |  2.97 |  29 |  282.8 | 2064.1 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   699.14 ms |  1.00 | 583 |  835.0 |  699.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   219.50 ms |  0.31 | 583 | 2659.6 |  219.5 |  268.09 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,181.98 ms |  1.69 | 583 |  493.9 | 1182.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,025.30 ms |  1.47 | 583 |  569.4 | 1025.3 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,104.55 ms |  3.01 | 583 |  277.4 | 2104.5 |  260.58 MB |        1.00 |
