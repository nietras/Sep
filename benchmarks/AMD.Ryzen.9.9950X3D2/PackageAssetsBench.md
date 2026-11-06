```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 44.57 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean       | Ratio | MB  | MB/s    | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-----------:|------:|----:|--------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |   1.377 ms |  1.00 |  29 | 21189.7 |   27.5 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |   1.432 ms |  1.04 |  29 | 20383.4 |   28.6 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |   1.423 ms |  1.03 |  29 | 20512.1 |   28.5 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |   2.005 ms |  1.46 |  29 | 14554.5 |   40.1 |      8657 B |       30.06 |
| CsvHelper    | Row   | 50000   |   1.857 ms |  1.35 |  29 | 15713.4 |   37.1 |      1264 B |        4.39 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Cols  | 50000   |   1.928 ms |  1.00 |  29 | 15133.5 |   38.6 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |   2.370 ms |  1.23 |  29 | 12315.2 |   47.4 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |   3.339 ms |  1.73 |  29 |  8739.0 |   66.8 |      8658 B |       30.06 |
| CsvHelper    | Cols  | 50000   |  10.434 ms |  5.41 |  29 |  2796.6 |  208.7 |   1262624 B |    4,384.11 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 50000   |  21.292 ms |  1.00 |  29 |  1370.5 |  425.8 |  14132452 B |        1.00 |
| Sep_MT___    | Asset | 50000   |   9.059 ms |  0.43 |  29 |  3221.2 |  181.2 |  14303172 B |        1.01 |
| Sylvan___    | Asset | 50000   |  26.211 ms |  1.23 |  29 |  1113.3 |  524.2 |  14296790 B |        1.01 |
| CsvHelper    | Asset | 50000   |  23.481 ms |  1.10 |  29 |  1242.8 |  469.6 |  15111670 B |        1.07 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 1000000 | 407.611 ms |  1.00 | 583 |  1432.2 |  407.6 | 273063040 B |        1.00 |
| Sep_MT___    | Asset | 1000000 | 172.622 ms |  0.42 | 583 |  3381.9 |  172.6 | 274161200 B |        1.00 |
| Sylvan___    | Asset | 1000000 | 487.475 ms |  1.20 | 583 |  1197.6 |  487.5 | 273226592 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 452.330 ms |  1.11 | 583 |  1290.6 |  452.3 | 274041952 B |        1.00 |
