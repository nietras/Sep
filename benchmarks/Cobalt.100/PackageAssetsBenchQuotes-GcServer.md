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
| Sep______ | Asset | 50000   |    38.74 ms |  1.00 |  33 |  861.7 |  774.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    14.33 ms |  0.37 |  33 | 2328.9 |  286.6 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    56.17 ms |  1.45 |  33 |  594.2 | 1123.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    52.81 ms |  1.36 |  33 |  632.0 | 1056.3 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   106.48 ms |  2.75 |  33 |  313.5 | 2129.6 |   13.65 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   753.75 ms |  1.00 | 667 |  885.8 |  753.8 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   240.60 ms |  0.32 | 667 | 2775.2 |  240.6 |  261.82 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,136.65 ms |  1.51 | 667 |  587.4 | 1136.7 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,173.52 ms |  1.56 | 667 |  569.0 | 1173.5 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,157.21 ms |  2.86 | 667 |  309.5 | 2157.2 |  260.58 MB |        1.00 |
