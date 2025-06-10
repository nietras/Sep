```

BenchmarkDotNet v0.15.1, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-DUYDYG : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.587 ms |  1.00 | 20 | 7835.1 |  103.5 |     1.22 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  18.138 ms |  7.04 | 20 | 1117.6 |  725.5 |    10.37 KB |        8.52 |
| ReadLine_ | Row    | 25000 |  14.297 ms |  5.55 | 20 | 1417.9 |  571.9 | 73489.65 KB |   60,395.99 |
| CsvHelper | Row    | 25000 |  28.028 ms | 10.87 | 20 |  723.3 | 1121.1 |    20.28 KB |       16.66 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   3.300 ms |  1.00 | 20 | 6142.5 |  132.0 |     1.22 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  20.824 ms |  6.31 | 20 |  973.5 |  833.0 |    10.62 KB |        8.72 |
| ReadLine_ | Cols   | 25000 |  15.303 ms |  4.64 | 20 | 1324.7 |  612.1 | 73489.65 KB |   60,395.99 |
| CsvHelper | Cols   | 25000 |  29.720 ms |  9.01 | 20 |  682.1 | 1188.8 |  21340.5 KB |   17,538.26 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  23.364 ms |  1.00 | 20 |  867.6 |  934.6 |     8.32 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   8.824 ms |  0.38 | 20 | 2297.4 |  353.0 |    59.29 KB |        7.13 |
| Sylvan___ | Floats | 25000 |  69.662 ms |  2.98 | 20 |  291.0 | 2786.5 |    18.57 KB |        2.23 |
| ReadLine_ | Floats | 25000 |  79.703 ms |  3.41 | 20 |  254.3 | 3188.1 |  73493.2 KB |    8,832.99 |
| CsvHelper | Floats | 25000 | 107.176 ms |  4.59 | 20 |  189.1 | 4287.1 | 22063.34 KB |    2,651.74 |
