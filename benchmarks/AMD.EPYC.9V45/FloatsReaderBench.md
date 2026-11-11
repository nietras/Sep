```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD EPYC 9V45 4.06GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.5 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|--------:|-------:|----------:|------------:|
| Sep______ | Row    | 25000 |  1.482 ms |  1.00 | 20 | 13675.4 |   59.3 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  2.571 ms |  1.74 | 20 |  7884.0 |  102.9 |  16.94 KB |        4.44 |
| CsvHelper | Row    | 25000 |  1.894 ms |  1.28 | 20 | 10703.5 |   75.8 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |         |        |           |             |
| Sep______ | Cols   | 25000 |  1.999 ms |  1.00 | 20 | 10140.8 |   80.0 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  3.886 ms |  1.94 | 20 |  5217.3 |  155.4 |  16.94 KB |        4.44 |
| CsvHelper | Cols   | 25000 |  2.507 ms |  1.25 | 20 |  8086.0 |  100.3 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |         |        |           |             |
| Sep______ | Floats | 25000 | 20.886 ms |  1.00 | 20 |   970.6 |  835.5 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  8.639 ms |  0.41 | 20 |  2346.7 |  345.5 |  54.08 KB |        9.24 |
| Sylvan___ | Floats | 25000 | 43.062 ms |  2.06 | 20 |   470.8 | 1722.5 |   18.6 KB |        3.18 |
| CsvHelper | Floats | 25000 | 21.695 ms |  1.04 | 20 |   934.4 |  867.8 |   6.09 KB |        1.04 |
