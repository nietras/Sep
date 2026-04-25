```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7184/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |  17.929 ms |  1.00 |  33 | 1861.7 |  358.6 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   5.872 ms |  0.33 |  33 | 5684.4 |  117.4 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  26.385 ms |  1.47 |  33 | 1265.0 |  527.7 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |  21.798 ms |  1.22 |  33 | 1531.2 |  436.0 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |  47.041 ms |  2.62 |  33 |  709.5 |  940.8 |   13.64 MB |        1.01 |
|           |       |         |            |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 | 358.149 ms |  1.00 | 667 | 1864.3 |  358.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 105.507 ms |  0.29 | 667 | 6328.6 |  105.5 |  261.97 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 542.348 ms |  1.51 | 667 | 1231.1 |  542.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 399.659 ms |  1.12 | 667 | 1670.7 |  399.7 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 942.707 ms |  2.63 | 667 |  708.3 |  942.7 |  260.58 MB |        1.00 |
