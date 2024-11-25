```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.1 (23H222) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  Job-OMZVHU : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD

Job=Job-OMZVHU  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    26.67 ms |  1.00 |  29 | 1090.7 |  533.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    20.88 ms |  0.78 |  29 | 1393.1 |  417.6 |   13.66 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    51.56 ms |  1.93 |  29 |  564.2 | 1031.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    44.37 ms |  1.67 |  29 |  655.5 |  887.5 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    80.94 ms |  3.04 |  29 |  359.4 | 1618.7 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   547.04 ms |  1.00 | 581 | 1063.7 |  547.0 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   227.53 ms |  0.42 | 581 | 2557.4 |  227.5 |  266.03 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,058.09 ms |  1.93 | 581 |  549.9 | 1058.1 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,105.75 ms |  2.02 | 581 |  526.2 | 1105.8 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,588.28 ms |  2.90 | 581 |  366.4 | 1588.3 |  260.58 MB |        1.00 |
