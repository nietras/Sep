```

BenchmarkDotNet v0.15.6, Windows 10 (10.0.19045.6575/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |  17.792 ms |  1.00 |  33 | 1876.0 |  355.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   5.863 ms |  0.33 |  33 | 5692.8 |  117.3 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  27.020 ms |  1.52 |  33 | 1235.3 |  540.4 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |  22.191 ms |  1.25 |  33 | 1504.1 |  443.8 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |  46.946 ms |  2.64 |  33 |  711.0 |  938.9 |   13.65 MB |        1.01 |
|           |       |         |            |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 | 363.921 ms |  1.00 | 667 | 1834.8 |  363.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 104.427 ms |  0.29 | 667 | 6394.0 |  104.4 |  261.51 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 541.503 ms |  1.49 | 667 | 1233.1 |  541.5 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 422.719 ms |  1.16 | 667 | 1579.6 |  422.7 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 938.489 ms |  2.58 | 667 |  711.5 |  938.5 |  260.58 MB |        1.00 |
