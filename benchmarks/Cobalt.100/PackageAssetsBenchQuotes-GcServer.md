```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8246/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    39.19 ms |  1.00 |  33 |  851.7 |  783.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    14.58 ms |  0.37 |  33 | 2288.5 |  291.7 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    56.84 ms |  1.45 |  33 |  587.2 | 1136.7 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    54.70 ms |  1.40 |  33 |  610.2 | 1093.9 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   106.86 ms |  2.73 |  33 |  312.4 | 2137.1 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   756.58 ms |  1.00 | 667 |  882.5 |  756.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   241.37 ms |  0.32 | 667 | 2766.3 |  241.4 |  263.15 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,136.67 ms |  1.50 | 667 |  587.4 | 1136.7 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,220.66 ms |  1.61 | 667 |  547.0 | 1220.7 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,127.85 ms |  2.81 | 667 |  313.8 | 2127.8 |  260.58 MB |        1.00 |
