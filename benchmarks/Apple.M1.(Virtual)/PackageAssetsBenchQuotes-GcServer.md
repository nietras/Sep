```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.1 (23H222) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  Job-OMZVHU : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD

Job=Job-OMZVHU  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.36 ms |  1.00 |  33 | 1028.5 |  647.2 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    14.74 ms |  0.46 |  33 | 2258.4 |  294.7 |   13.59 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    52.76 ms |  1.63 |  33 |  630.8 | 1055.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    40.49 ms |  1.25 |  33 |  821.9 |  809.9 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    84.67 ms |  2.62 |  33 |  393.1 | 1693.3 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   664.17 ms |  1.00 | 665 | 1002.5 |  664.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   353.26 ms |  0.53 | 665 | 1884.7 |  353.3 |  267.27 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,082.34 ms |  1.63 | 665 |  615.1 | 1082.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,249.34 ms |  1.88 | 665 |  532.9 | 1249.3 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,598.74 ms |  2.41 | 665 |  416.5 | 1598.7 |  260.58 MB |        1.00 |
