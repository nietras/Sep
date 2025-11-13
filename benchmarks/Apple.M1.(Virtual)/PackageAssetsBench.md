```

BenchmarkDotNet v0.15.6, macOS Sequoia 15.7.1 (24G231) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     3.972 ms |  1.01 |  29 | 7323.2 |   79.4 |        952 B |        1.00 |
| Sep_Async    | Row   | 50000   |     3.992 ms |  1.01 |  29 | 7286.9 |   79.8 |        952 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.902 ms |  1.24 |  29 | 5933.7 |   98.0 |        952 B |        1.00 |
| Sylvan___    | Row   | 50000   |    27.633 ms |  7.01 |  29 | 1052.6 |  552.7 |       6692 B |        7.03 |
| ReadLine_    | Row   | 50000   |    21.068 ms |  5.34 |  29 | 1380.6 |  421.4 |   90734824 B |   95,309.69 |
| CsvHelper    | Row   | 50000   |    58.651 ms | 14.88 |  29 |  495.9 | 1173.0 |      20424 B |       21.45 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     4.505 ms |  1.00 |  29 | 6456.1 |   90.1 |        952 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.077 ms |  1.13 |  29 | 5729.1 |  101.5 |        952 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    29.637 ms |  6.58 |  29 |  981.4 |  592.7 |       6692 B |        7.03 |
| ReadLine_    | Cols  | 50000   |    25.961 ms |  5.77 |  29 | 1120.4 |  519.2 |   90734824 B |   95,309.69 |
| CsvHelper    | Cols  | 50000   |    70.982 ms | 15.76 |  29 |  409.8 | 1419.6 |     456296 B |      479.30 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    39.331 ms |  1.01 |  29 |  739.5 |  786.6 |   14133358 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    32.270 ms |  0.83 |  29 |  901.3 |  645.4 |   14228814 B |        1.01 |
| Sylvan___    | Asset | 50000   |    58.127 ms |  1.49 |  29 |  500.4 | 1162.5 |   14295768 B |        1.01 |
| ReadLine_    | Asset | 50000   |   115.274 ms |  2.95 |  29 |  252.3 | 2305.5 |  104585064 B |        7.40 |
| CsvHelper    | Asset | 50000   |    92.185 ms |  2.36 |  29 |  315.5 | 1843.7 |   14305450 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   752.281 ms |  1.00 | 581 |  773.5 |  752.3 |  273067192 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   804.179 ms |  1.07 | 581 |  723.6 |  804.2 |  287567112 B |        1.05 |
| Sylvan___    | Asset | 1000000 | 1,410.084 ms |  1.88 | 581 |  412.7 | 1410.1 |  273226864 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,592.746 ms |  4.79 | 581 |  162.0 | 3592.7 | 2087766752 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,322.007 ms |  3.10 | 581 |  250.6 | 2322.0 |  273241184 B |        1.00 |
