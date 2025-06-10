```

BenchmarkDotNet v0.15.1, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-SNAXSU : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-SNAXSU  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    25.60 ms |  1.00 |  29 | 1136.3 |  512.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    18.61 ms |  0.73 |  29 | 1562.6 |  372.3 |   13.57 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    66.50 ms |  2.61 |  29 |  437.4 | 1330.0 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    53.19 ms |  2.08 |  29 |  546.9 | 1063.8 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    91.97 ms |  3.60 |  29 |  316.2 | 1839.5 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   594.24 ms |  1.02 | 581 |  979.2 |  594.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   300.93 ms |  0.51 | 581 | 1933.6 |  300.9 |  271.08 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   974.34 ms |  1.66 | 581 |  597.2 |  974.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,102.38 ms |  1.88 | 581 |  527.8 | 1102.4 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,547.16 ms |  2.64 | 581 |  376.1 | 1547.2 |  260.58 MB |        1.00 |
