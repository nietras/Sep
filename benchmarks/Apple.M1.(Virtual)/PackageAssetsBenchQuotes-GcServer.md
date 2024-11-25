```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.1 (23H222) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  Job-FTLBYL : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD

Job=Job-FTLBYL  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.76 ms |  1.00 |  33 | 1016.0 |  655.1 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.71 ms |  0.48 |  33 | 2118.6 |  314.2 |   13.52 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    51.34 ms |  1.57 |  33 |  648.2 | 1026.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    38.24 ms |  1.17 |  33 |  870.4 |  764.8 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    77.91 ms |  2.38 |  33 |  427.2 | 1558.2 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   616.92 ms |  1.00 | 665 | 1079.2 |  616.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   325.41 ms |  0.53 | 665 | 2046.0 |  325.4 |  261.58 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,016.69 ms |  1.65 | 665 |  654.9 | 1016.7 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,283.65 ms |  2.08 | 665 |  518.7 | 1283.7 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,603.01 ms |  2.60 | 665 |  415.3 | 1603.0 |  260.58 MB |        1.00 |
