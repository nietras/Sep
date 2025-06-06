```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-JAYDZK : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-JAYDZK  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    22.492 ms |  1.00 |  29 | 1293.2 |  449.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     9.970 ms |  0.44 |  29 | 2917.4 |  199.4 |    13.6 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    46.969 ms |  2.09 |  29 |  619.3 |  939.4 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    39.353 ms |  1.75 |  29 |  739.1 |  787.1 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    72.682 ms |  3.23 |  29 |  400.2 | 1453.6 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   466.490 ms |  1.00 | 581 | 1247.3 |  466.5 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   193.211 ms |  0.41 | 581 | 3011.6 |  193.2 |  271.09 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   958.321 ms |  2.05 | 581 |  607.2 |  958.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,113.932 ms |  2.39 | 581 |  522.4 | 1113.9 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,582.009 ms |  3.39 | 581 |  367.8 | 1582.0 |  260.58 MB |        1.00 |
