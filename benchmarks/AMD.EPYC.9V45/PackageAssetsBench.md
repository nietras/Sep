```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD EPYC 9V45 4.06GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.5 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean       | Ratio | MB  | MB/s    | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-----------:|------:|----:|--------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |   2.190 ms |  1.00 |  29 | 13282.3 |   43.8 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |   2.216 ms |  1.01 |  29 | 13122.8 |   44.3 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |   2.456 ms |  1.12 |  29 | 11843.9 |   49.1 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |   3.137 ms |  1.43 |  29 |  9271.7 |   62.7 |      8658 B |       30.06 |
| CsvHelper    | Row   | 50000   |   3.034 ms |  1.39 |  29 |  9586.5 |   60.7 |      1264 B |        4.39 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Cols  | 50000   |   2.979 ms |  1.00 |  29 |  9762.7 |   59.6 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |   4.042 ms |  1.36 |  29 |  7195.5 |   80.8 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |   5.006 ms |  1.68 |  29 |  5810.5 |  100.1 |      8659 B |       30.07 |
| CsvHelper    | Cols  | 50000   |  13.224 ms |  4.44 |  29 |  2199.5 |  264.5 |   1262624 B |    4,384.11 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 50000   |  32.328 ms |  1.00 |  29 |   899.7 |  646.6 |  14132904 B |        1.00 |
| Sep_MT___    | Asset | 50000   |  20.326 ms |  0.63 |  29 |  1431.0 |  406.5 |  14215396 B |        1.01 |
| Sylvan___    | Asset | 50000   |  36.167 ms |  1.12 |  29 |   804.2 |  723.3 |  14297144 B |        1.01 |
| CsvHelper    | Asset | 50000   |  33.227 ms |  1.03 |  29 |   875.4 |  664.5 |  15112094 B |        1.07 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 1000000 | 601.113 ms |  1.00 | 581 |   968.0 |  601.1 | 273071528 B |        1.00 |
| Sep_MT___    | Asset | 1000000 | 392.623 ms |  0.65 | 581 |  1482.0 |  392.6 | 283671072 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 720.689 ms |  1.20 | 581 |   807.4 |  720.7 | 273232912 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 638.392 ms |  1.06 | 581 |   911.5 |  638.4 | 274045672 B |        1.00 |
