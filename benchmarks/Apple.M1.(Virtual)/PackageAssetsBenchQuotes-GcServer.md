```

BenchmarkDotNet v0.15.1, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-HVKXWJ : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-HVKXWJ  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    28.16 ms |  1.00 |  33 | 1182.0 |  563.1 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    12.56 ms |  0.45 |  33 | 2649.1 |  251.3 |   13.62 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    49.05 ms |  1.74 |  33 |  678.5 |  981.0 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    37.15 ms |  1.32 |  33 |  895.9 |  743.0 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    78.83 ms |  2.80 |  33 |  422.2 | 1576.7 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   556.88 ms |  1.00 | 665 | 1195.6 |  556.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   270.95 ms |  0.49 | 665 | 2457.2 |  271.0 |  266.35 MB |        1.02 |
| Sylvan___ | Asset | 1000000 |   997.69 ms |  1.79 | 665 |  667.3 |  997.7 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,353.30 ms |  2.43 | 665 |  492.0 | 1353.3 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,976.51 ms |  3.55 | 665 |  336.9 | 1976.5 |  260.58 MB |        1.00 |
