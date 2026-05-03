```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     3.461 ms |  1.00 |  29 | 8403.6 |   69.2 |        968 B |        1.00 |
| Sep_Async    | Row   | 50000   |     4.297 ms |  1.24 |  29 | 6768.2 |   85.9 |        968 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.465 ms |  1.00 |  29 | 8394.9 |   69.3 |        968 B |        1.00 |
| Sylvan___    | Row   | 50000   |    19.685 ms |  5.69 |  29 | 1477.6 |  393.7 |       7476 B |        7.72 |
| ReadLine_    | Row   | 50000   |    18.590 ms |  5.38 |  29 | 1564.6 |  371.8 |   90734824 B |   93,734.32 |
| CsvHelper    | Row   | 50000   |    46.406 ms | 13.42 |  29 |  626.8 |  928.1 |      20424 B |       21.10 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     4.360 ms |  1.00 |  29 | 6671.6 |   87.2 |        968 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.203 ms |  1.19 |  29 | 5590.6 |  104.1 |        968 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.153 ms |  5.32 |  29 | 1256.2 |  463.1 |       7516 B |        7.76 |
| ReadLine_    | Cols  | 50000   |    20.198 ms |  4.64 |  29 | 1440.0 |  404.0 |   90734824 B |   93,734.32 |
| CsvHelper    | Cols  | 50000   |    69.849 ms | 16.04 |  29 |  416.4 | 1397.0 |     456296 B |      471.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    35.358 ms |  1.01 |  29 |  822.6 |  707.2 |   14133416 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    24.464 ms |  0.70 |  29 | 1188.9 |  489.3 |   14446080 B |        1.02 |
| Sylvan___    | Asset | 50000   |    52.882 ms |  1.51 |  29 |  550.0 | 1057.6 |   14295914 B |        1.01 |
| ReadLine_    | Asset | 50000   |    85.798 ms |  2.45 |  29 |  339.0 | 1716.0 |  104585278 B |        7.40 |
| CsvHelper    | Asset | 50000   |    82.423 ms |  2.35 |  29 |  352.9 | 1648.5 |   14305304 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   623.531 ms |  1.00 | 581 |  933.2 |  623.5 |  273067184 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   497.698 ms |  0.80 | 581 | 1169.1 |  497.7 |  285459648 B |        1.05 |
| Sylvan___    | Asset | 1000000 | 1,044.701 ms |  1.68 | 581 |  557.0 | 1044.7 |  273226744 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,665.312 ms |  2.67 | 581 |  349.4 | 1665.3 | 2087767352 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,556.015 ms |  2.50 | 581 |  374.0 | 1556.0 |  273236192 B |        1.00 |
