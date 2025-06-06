```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3)
Cobalt 100, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-TACNXO : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-TACNXO  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    41.76 ms |  1.00 |  33 |  799.2 |  835.2 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.20 ms |  0.36 |  33 | 2195.6 |  304.0 |   13.57 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    63.00 ms |  1.51 |  33 |  529.8 | 1260.0 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    54.62 ms |  1.31 |  33 |  611.1 | 1092.4 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   111.38 ms |  2.67 |  33 |  299.7 | 2227.6 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   822.72 ms |  1.00 | 667 |  811.6 |  822.7 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   276.76 ms |  0.34 | 667 | 2412.6 |  276.8 |  262.36 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,245.58 ms |  1.51 | 667 |  536.1 | 1245.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,261.25 ms |  1.53 | 667 |  529.4 | 1261.2 | 2385.08 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,224.92 ms |  2.70 | 667 |  300.1 | 2224.9 |  260.58 MB |        1.00 |
