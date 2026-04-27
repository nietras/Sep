```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     7.013 ms |  1.00 |  33 | 4745.6 |  140.3 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     7.051 ms |  1.01 |  33 | 4720.0 |  141.0 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     7.406 ms |  1.06 |  33 | 4494.0 |  148.1 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    20.310 ms |  2.90 |  33 | 1638.7 |  406.2 |       7476 B |        7.79 |
| ReadLine_    | Row   | 50000   |    19.414 ms |  2.77 |  33 | 1714.3 |  388.3 |  111389416 B |  116,030.64 |
| CsvHelper    | Row   | 50000   |    46.644 ms |  6.65 |  33 |  713.5 |  932.9 |      20496 B |       21.35 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     8.052 ms |  1.00 |  33 | 4133.4 |  161.0 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     9.169 ms |  1.14 |  33 | 3629.8 |  183.4 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.342 ms |  2.90 |  33 | 1425.9 |  466.8 |       7516 B |        7.83 |
| ReadLine_    | Cols  | 50000   |    19.795 ms |  2.46 |  33 | 1681.4 |  395.9 |  111389416 B |  116,030.64 |
| CsvHelper    | Cols  | 50000   |    70.317 ms |  8.73 |  33 |  473.3 | 1406.3 |     456368 B |      475.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    34.288 ms |  1.00 |  33 |  970.7 |  685.8 |   14133366 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    21.354 ms |  0.62 |  33 | 1558.6 |  427.1 |   14295653 B |        1.01 |
| Sylvan___    | Asset | 50000   |    53.319 ms |  1.56 |  33 |  624.2 | 1066.4 |   14295958 B |        1.01 |
| ReadLine_    | Asset | 50000   |   118.383 ms |  3.46 |  33 |  281.1 | 2367.7 |  125239978 B |        8.86 |
| CsvHelper    | Asset | 50000   |    79.573 ms |  2.33 |  33 |  418.3 | 1591.5 |   14305304 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   725.199 ms |  1.00 | 665 |  918.1 |  725.2 |  273067136 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   463.545 ms |  0.64 | 665 | 1436.3 |  463.5 |  282125760 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,118.727 ms |  1.54 | 665 |  595.1 | 1118.7 |  273226824 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,287.849 ms |  3.16 | 665 |  291.0 | 2287.8 | 2500934080 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,736.191 ms |  2.40 | 665 |  383.5 | 1736.2 |  273240248 B |        1.00 |
