```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.4 (23H420) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-OHNQXW : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-OHNQXW  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    29.44 ms |  1.01 |  29 |  988.1 |  588.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    19.53 ms |  0.67 |  29 | 1489.2 |  390.6 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    53.48 ms |  1.83 |  29 |  543.9 | 1069.6 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    45.04 ms |  1.54 |  29 |  645.7 |  900.9 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    74.14 ms |  2.53 |  29 |  392.3 | 1482.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   495.08 ms |  1.00 | 581 | 1175.3 |  495.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   200.23 ms |  0.40 | 581 | 2906.0 |  200.2 |  268.19 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   965.53 ms |  1.95 | 581 |  602.7 |  965.5 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,068.55 ms |  2.16 | 581 |  544.5 | 1068.5 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,518.90 ms |  3.07 | 581 |  383.1 | 1518.9 |  260.58 MB |        1.00 |
