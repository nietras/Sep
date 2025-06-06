```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3)
Cobalt 100, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-MYYDFG : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-MYYDFG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  13.20 ms |  1.00 | 41 | 3164.8 |  264.0 |     995 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  17.76 ms |  1.35 | 41 | 2352.5 |  355.1 |    1011 B |        1.02 |
| Sep_TrimUnescape           | Cols  | 50000 |  18.55 ms |  1.41 | 41 | 2251.3 |  371.1 |    1014 B |        1.02 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.56 ms |  1.56 | 41 | 2032.1 |  411.1 |    1019 B |        1.02 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 119.81 ms |  9.08 | 41 |  348.6 | 2396.1 |  465358 B |      467.70 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 117.70 ms |  8.92 | 41 |  354.9 | 2354.0 |  456396 B |      458.69 |
