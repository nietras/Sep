```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8246/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
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
| Sep______    | Row   | 50000   |    10.207 ms |  1.00 |  33 | 3270.1 |  204.1 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |    10.206 ms |  1.00 |  33 | 3270.4 |  204.1 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     9.896 ms |  0.97 |  33 | 3373.0 |  197.9 |        961 B |        1.00 |
| Sylvan___    | Row   | 50000   |    22.452 ms |  2.20 |  33 | 1486.6 |  449.0 |       7478 B |        7.79 |
| ReadLine_    | Row   | 50000   |    23.521 ms |  2.30 |  33 | 1419.0 |  470.4 |  111389419 B |  116,030.64 |
| CsvHelper    | Row   | 50000   |    62.571 ms |  6.13 |  33 |  533.4 | 1251.4 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    11.354 ms |  1.00 |  33 | 2939.7 |  227.1 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.050 ms |  1.15 |  33 | 2557.7 |  261.0 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    27.160 ms |  2.39 |  33 | 1228.9 |  543.2 |       7481 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    23.777 ms |  2.09 |  33 | 1403.7 |  475.5 |  111389416 B |  116,030.64 |
| CsvHelper    | Cols  | 50000   |    95.159 ms |  8.38 |  33 |  350.8 | 1903.2 |     456368 B |      475.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    36.395 ms |  1.00 |  33 |  917.1 |  727.9 |   14133000 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    14.357 ms |  0.39 |  33 | 2324.7 |  287.1 |   14192866 B |        1.00 |
| Sylvan___    | Asset | 50000   |    58.207 ms |  1.60 |  33 |  573.4 | 1164.1 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   109.343 ms |  3.00 |  33 |  305.3 | 2186.9 |  125238788 B |        8.86 |
| CsvHelper    | Asset | 50000   |   105.715 ms |  2.90 |  33 |  315.7 | 2114.3 |   14305310 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   902.368 ms |  1.00 | 667 |  739.9 |  902.4 |  273063624 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   369.414 ms |  0.41 | 667 | 1807.5 |  369.4 |  281095576 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,286.646 ms |  1.43 | 667 |  519.0 | 1286.6 |  273225288 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,910.421 ms |  3.23 | 667 |  229.4 | 2910.4 | 2500932184 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,280.425 ms |  2.53 | 667 |  292.8 | 2280.4 |  273238720 B |        1.00 |
