```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3)
Cobalt 100, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-TACNXO : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-TACNXO  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    35.08 ms |  1.00 |  29 |  831.8 |  701.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.91 ms |  0.34 |  29 | 2450.9 |  238.1 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    59.02 ms |  1.68 |  29 |  494.5 | 1180.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    46.68 ms |  1.33 |  29 |  625.1 |  933.7 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   104.57 ms |  2.98 |  29 |  279.1 | 2091.3 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   704.38 ms |  1.00 | 583 |  828.8 |  704.4 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   219.52 ms |  0.31 | 583 | 2659.4 |  219.5 |  268.18 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,188.95 ms |  1.69 | 583 |  491.0 | 1189.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,058.96 ms |  1.50 | 583 |  551.3 | 1059.0 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,103.12 ms |  2.99 | 583 |  277.6 | 2103.1 |  260.58 MB |        1.00 |
