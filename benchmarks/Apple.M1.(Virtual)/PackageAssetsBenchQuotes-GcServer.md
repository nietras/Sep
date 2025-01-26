```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.2 (23H311) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD
  Job-EAJOLX : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD

Job=Job-EAJOLX  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    30.10 ms |  1.00 |  33 | 1105.7 |  602.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.30 ms |  0.51 |  33 | 2175.0 |  306.0 |   13.52 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    50.82 ms |  1.69 |  33 |  654.9 | 1016.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    38.82 ms |  1.29 |  33 |  857.3 |  776.4 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    78.92 ms |  2.62 |  33 |  421.7 | 1578.4 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   619.24 ms |  1.00 | 665 | 1075.2 |  619.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   310.66 ms |  0.50 | 665 | 2143.2 |  310.7 |  261.09 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,014.27 ms |  1.64 | 665 |  656.4 | 1014.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,283.69 ms |  2.07 | 665 |  518.7 | 1283.7 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,622.24 ms |  2.62 | 665 |  410.4 | 1622.2 |  260.58 MB |        1.00 |
