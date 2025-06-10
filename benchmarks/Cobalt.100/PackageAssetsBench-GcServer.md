```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-IRVJCV : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-IRVJCV  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    35.20 ms |  1.00 |  29 |  829.1 |  703.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.73 ms |  0.33 |  29 | 2487.2 |  234.7 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    59.11 ms |  1.68 |  29 |  493.7 | 1182.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    46.14 ms |  1.31 |  29 |  632.5 |  922.8 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   103.49 ms |  2.95 |  29 |  282.0 | 2069.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   704.67 ms |  1.00 | 583 |  828.4 |  704.7 |  260.42 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   220.47 ms |  0.31 | 583 | 2647.9 |  220.5 |  268.15 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,181.71 ms |  1.68 | 583 |  494.0 | 1181.7 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,023.90 ms |  1.45 | 583 |  570.2 | 1023.9 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,106.97 ms |  2.99 | 583 |  277.1 | 2107.0 |  260.58 MB |        1.00 |
