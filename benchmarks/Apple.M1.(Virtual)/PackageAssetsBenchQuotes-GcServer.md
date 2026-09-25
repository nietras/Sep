```

BenchmarkDotNet v0.16.0-preview.1, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
Memory: 7 GB Total, 0.09 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Server=True  Toolchain=net11.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    35.60 ms |  1.00 |  33 |  934.9 |  712.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.30 ms |  0.43 |  33 | 2176.0 |  305.9 |   13.58 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    71.86 ms |  2.03 |  33 |  463.1 | 1437.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    65.83 ms |  1.86 |  33 |  505.6 | 1316.5 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   102.34 ms |  2.89 |  33 |  325.2 | 2046.8 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   866.21 ms |  1.00 | 665 |  768.6 |  866.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   413.59 ms |  0.49 | 665 | 1609.8 |  413.6 |  268.96 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,292.04 ms |  1.52 | 665 |  515.3 | 1292.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 2,213.82 ms |  2.61 | 665 |  300.7 | 2213.8 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,217.86 ms |  2.62 | 665 |  300.2 | 2217.9 |  260.58 MB |        1.00 |
