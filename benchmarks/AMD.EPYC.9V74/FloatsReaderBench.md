```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.33 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.173 ms |  1.00 | 20 | 6388.0 |  126.9 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.789 ms |  1.19 | 20 | 5349.8 |  151.6 |     12.5 KB |       10.00 |
| ReadLine_ | Row    | 25000 |  18.400 ms |  5.80 | 20 | 1101.7 |  736.0 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 |  40.551 ms | 12.78 | 20 |  499.9 | 1622.1 |    19.95 KB |       15.96 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.436 ms |  1.00 | 20 | 4569.9 |  177.4 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.426 ms |  1.45 | 20 | 3154.6 |  257.1 |     12.5 KB |       10.00 |
| ReadLine_ | Cols   | 25000 |  18.301 ms |  4.13 | 20 | 1107.7 |  732.0 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 |  43.718 ms |  9.86 | 20 |  463.7 | 1748.7 | 21340.16 KB |   17,072.13 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  35.413 ms |  1.00 | 20 |  572.4 | 1416.5 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  15.341 ms |  0.43 | 20 | 1321.4 |  613.7 |    73.08 KB |        9.25 |
| Sylvan___ | Floats | 25000 |  83.844 ms |  2.37 | 20 |  241.8 | 3353.7 |    18.63 KB |        2.36 |
| ReadLine_ | Floats | 25000 | 112.182 ms |  3.17 | 20 |  180.7 | 4487.3 | 73492.93 KB |    9,304.74 |
| CsvHelper | Floats | 25000 | 160.399 ms |  4.53 | 20 |  126.4 | 6416.0 | 22061.55 KB |    2,793.15 |
