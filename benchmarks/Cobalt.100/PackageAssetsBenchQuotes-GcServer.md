```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-IRVJCV : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-IRVJCV  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    41.15 ms |  1.00 |  33 |  811.1 |  823.1 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    14.76 ms |  0.36 |  33 | 2260.9 |  295.3 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    62.71 ms |  1.53 |  33 |  532.2 | 1254.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    54.59 ms |  1.33 |  33 |  611.4 | 1091.9 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   110.54 ms |  2.69 |  33 |  302.0 | 2210.8 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   824.75 ms |  1.00 | 667 |  809.6 |  824.7 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   275.53 ms |  0.33 | 667 | 2423.4 |  275.5 |  262.46 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,247.45 ms |  1.51 | 667 |  535.3 | 1247.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,245.37 ms |  1.51 | 667 |  536.2 | 1245.4 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,224.39 ms |  2.70 | 667 |  300.2 | 2224.4 |  260.58 MB |        1.00 |
