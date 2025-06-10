```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-CWGIKZ : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-CWGIKZ  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    40.53 ms |  1.00 |  33 |  823.4 |  810.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    14.65 ms |  0.36 |  33 | 2278.6 |  293.0 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    62.73 ms |  1.55 |  33 |  532.1 | 1254.6 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    54.84 ms |  1.35 |  33 |  608.7 | 1096.7 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   110.48 ms |  2.73 |  33 |  302.1 | 2209.6 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   837.31 ms |  1.00 | 667 |  797.4 |  837.3 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   276.68 ms |  0.33 | 667 | 2413.3 |  276.7 |   261.4 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,245.45 ms |  1.49 | 667 |  536.1 | 1245.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,247.39 ms |  1.49 | 667 |  535.3 | 1247.4 | 2385.08 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,256.19 ms |  2.69 | 667 |  295.9 | 2256.2 |  260.58 MB |        1.00 |
