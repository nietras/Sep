```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 10.67 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     4.620 ms |  1.00 |  29 | 6295.4 |   92.4 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     4.684 ms |  1.01 |  29 | 6210.2 |   93.7 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.625 ms |  1.00 |  29 | 6289.4 |   92.5 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    18.395 ms |  3.98 |  29 | 1581.2 |  367.9 |      7470 B |       25.94 |
| CsvHelper    | Row   | 50000   |    11.363 ms |  2.46 |  29 | 2559.6 |  227.3 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     6.155 ms |  1.00 |  29 | 4725.5 |  123.1 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.713 ms |  1.09 |  29 | 4332.7 |  134.3 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.000 ms |  3.74 |  29 | 1264.6 |  460.0 |      7478 B |       25.97 |
| CsvHelper    | Cols  | 50000   |    27.551 ms |  4.48 |  29 | 1055.7 |  551.0 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    34.554 ms |  1.00 |  29 |  841.7 |  691.1 |  14132256 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    18.045 ms |  0.52 |  29 | 1611.9 |  360.9 |  14186090 B |        1.00 |
| Sylvan___    | Asset | 50000   |    56.141 ms |  1.62 |  29 |  518.1 | 1122.8 |  14295531 B |        1.01 |
| CsvHelper    | Asset | 50000   |    43.056 ms |  1.25 |  29 |  675.5 |  861.1 |  15111560 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   782.374 ms |  1.00 | 581 |  743.7 |  782.4 | 273063064 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   365.392 ms |  0.47 | 581 | 1592.5 |  365.4 | 281345720 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,224.224 ms |  1.56 | 581 |  475.3 | 1224.2 | 273225568 B |        1.00 |
| CsvHelper    | Asset | 1000000 |   945.773 ms |  1.21 | 581 |  615.2 |  945.8 | 274041960 B |        1.00 |
