```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.4 (23H420) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-OHNQXW : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-OHNQXW  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    30.08 ms |  1.00 |  33 | 1106.5 |  601.6 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.52 ms |  0.52 |  33 | 2144.4 |  310.4 |   13.52 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    54.35 ms |  1.81 |  33 |  612.4 | 1086.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    36.99 ms |  1.23 |  33 |  899.8 |  739.8 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    77.85 ms |  2.59 |  33 |  427.5 | 1556.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   630.57 ms |  1.00 | 665 | 1055.9 |  630.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   287.28 ms |  0.46 | 665 | 2317.6 |  287.3 |  260.89 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,020.17 ms |  1.62 | 665 |  652.6 | 1020.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,232.68 ms |  1.95 | 665 |  540.1 | 1232.7 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,631.39 ms |  2.59 | 665 |  408.1 | 1631.4 |  260.58 MB |        1.00 |
