```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.2 (23H311) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD
  Job-FNIGBL : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD

Job=Job-FNIGBL  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    41.29 ms |  1.03 |  33 |  806.0 |  825.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    31.46 ms |  0.78 |  33 | 1058.0 |  629.1 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    58.99 ms |  1.47 |  33 |  564.2 | 1179.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    57.46 ms |  1.43 |  33 |  579.2 | 1149.3 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    85.16 ms |  2.12 |  33 |  390.8 | 1703.2 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   696.42 ms |  1.00 | 665 |  956.0 |  696.4 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   529.92 ms |  0.76 | 665 | 1256.4 |  529.9 |  266.15 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,168.96 ms |  1.68 | 665 |  569.6 | 1169.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,593.69 ms |  2.29 | 665 |  417.8 | 1593.7 | 2385.08 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,663.19 ms |  2.39 | 665 |  400.3 | 1663.2 |  260.58 MB |        1.00 |
