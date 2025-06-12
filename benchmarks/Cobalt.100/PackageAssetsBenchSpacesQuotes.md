```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-FXNRMG : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-FXNRMG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.76 ms |  1.00 | 41 | 3273.0 |  255.2 |     954 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  17.18 ms |  1.35 | 41 | 2430.8 |  343.7 |     955 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  18.28 ms |  1.43 | 41 | 2284.9 |  365.6 |    1698 B |        1.78 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.09 ms |  1.57 | 41 | 2079.4 |  401.8 |     977 B |        1.02 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 118.88 ms |  9.32 | 41 |  351.4 | 2377.6 |  462286 B |      484.58 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 116.82 ms |  9.15 | 41 |  357.5 | 2336.5 |  459642 B |      481.81 |
