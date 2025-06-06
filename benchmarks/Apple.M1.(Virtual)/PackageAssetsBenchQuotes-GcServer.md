```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-JAYDZK : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-JAYDZK  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    29.13 ms |  1.00 |  33 | 1142.7 |  582.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    12.21 ms |  0.42 |  33 | 2725.4 |  244.2 |    13.6 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    49.92 ms |  1.72 |  33 |  666.7 |  998.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    49.39 ms |  1.70 |  33 |  673.9 |  987.7 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    82.71 ms |  2.84 |  33 |  402.4 | 1654.3 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   570.22 ms |  1.00 | 665 | 1167.6 |  570.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   276.03 ms |  0.48 | 665 | 2412.0 |  276.0 |  267.18 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,126.59 ms |  1.98 | 665 |  591.0 | 1126.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,733.82 ms |  3.04 | 665 |  384.0 | 1733.8 | 2385.08 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,895.91 ms |  3.33 | 665 |  351.2 | 1895.9 |  260.58 MB |        1.00 |
