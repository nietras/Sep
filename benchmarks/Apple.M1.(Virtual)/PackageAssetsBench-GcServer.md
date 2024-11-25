```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.1 (23H222) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  Job-FTLBYL : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD

Job=Job-FTLBYL  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    28.39 ms |  1.01 |  29 | 1024.7 |  567.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    12.27 ms |  0.43 |  29 | 2369.8 |  245.5 |    13.6 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    49.88 ms |  1.77 |  29 |  583.1 |  997.6 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    39.40 ms |  1.40 |  29 |  738.2 |  788.1 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    80.40 ms |  2.85 |  29 |  361.8 | 1608.1 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   540.81 ms |  1.00 | 581 | 1075.9 |  540.8 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   222.93 ms |  0.41 | 581 | 2610.1 |  222.9 |  267.19 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,059.36 ms |  1.96 | 581 |  549.3 | 1059.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,112.97 ms |  2.06 | 581 |  522.8 | 1113.0 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,632.36 ms |  3.02 | 581 |  356.5 | 1632.4 |  260.58 MB |        1.00 |
