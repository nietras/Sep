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
| Sep______    | Row   | 50000   |     7.813 ms |  1.00 |  33 | 4259.8 |  156.3 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     7.173 ms |  0.92 |  33 | 4639.8 |  143.5 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     7.239 ms |  0.93 |  33 | 4597.9 |  144.8 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    20.816 ms |  2.67 |  33 | 1598.9 |  416.3 |       7476 B |        7.79 |
| ReadLine_    | Row   | 50000   |    19.778 ms |  2.54 |  33 | 1682.8 |  395.6 |  111389416 B |  116,030.64 |
| CsvHelper    | Row   | 50000   |    49.004 ms |  6.29 |  33 |  679.2 |  980.1 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     8.431 ms |  1.00 |  33 | 3947.4 |  168.6 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     9.362 ms |  1.11 |  33 | 3555.0 |  187.2 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    24.633 ms |  2.92 |  33 | 1351.1 |  492.7 |       7516 B |        7.83 |
| ReadLine_    | Cols  | 50000   |    21.459 ms |  2.55 |  33 | 1551.0 |  429.2 |  111389416 B |  116,030.64 |
| CsvHelper    | Cols  | 50000   |    71.579 ms |  8.49 |  33 |  465.0 | 1431.6 |     456296 B |      475.31 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    35.419 ms |  1.00 |  33 |  939.7 |  708.4 |   14134780 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    20.533 ms |  0.58 |  33 | 1620.9 |  410.7 |   14266359 B |        1.01 |
| Sylvan___    | Asset | 50000   |    52.107 ms |  1.48 |  33 |  638.7 | 1042.1 |   14295906 B |        1.01 |
| ReadLine_    | Asset | 50000   |   123.658 ms |  3.50 |  33 |  269.1 | 2473.2 |  125239978 B |        8.86 |
| CsvHelper    | Asset | 50000   |    81.827 ms |  2.32 |  33 |  406.7 | 1636.5 |   14305304 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   777.866 ms |  1.00 | 665 |  855.9 |  777.9 |  273067128 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   512.812 ms |  0.66 | 665 | 1298.3 |  512.8 |  283359352 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,348.335 ms |  1.74 | 665 |  493.8 | 1348.3 |  273229672 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,459.969 ms |  4.46 | 665 |  192.4 | 3460.0 | 2500934560 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,170.602 ms |  2.80 | 665 |  306.7 | 2170.6 |  273241416 B |        1.00 |
