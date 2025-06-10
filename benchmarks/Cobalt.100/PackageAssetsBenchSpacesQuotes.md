```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-ZAPULK : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-ZAPULK  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.87 ms |  1.00 | 41 | 3245.0 |  257.4 |     955 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  17.46 ms |  1.36 | 41 | 2391.8 |  349.3 |     991 B |        1.04 |
| Sep_TrimUnescape           | Cols  | 50000 |  18.29 ms |  1.42 | 41 | 2283.2 |  365.9 |    1030 B |        1.08 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.44 ms |  1.59 | 41 | 2043.7 |  408.8 |    1019 B |        1.07 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 118.79 ms |  9.24 | 41 |  351.6 | 2375.7 |  462358 B |      484.14 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 116.76 ms |  9.08 | 41 |  357.8 | 2335.1 |  456558 B |      478.07 |
