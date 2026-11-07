```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 52.22 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|--------:|-------:|----------:|------------:|
| Sep______ | Row    | 25000 |  1.025 ms |  1.00 | 20 | 19818.0 |   41.0 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1.659 ms |  1.62 | 20 | 12245.4 |   66.4 |  16.94 KB |        4.44 |
| CsvHelper | Row    | 25000 |  1.080 ms |  1.05 | 20 | 18809.4 |   43.2 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |         |        |           |             |
| Sep______ | Cols   | 25000 |  1.452 ms |  1.00 | 20 | 13995.1 |   58.1 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2.587 ms |  1.78 | 20 |  7853.4 |  103.5 |  16.94 KB |        4.44 |
| CsvHelper | Cols   | 25000 |  1.565 ms |  1.08 | 20 | 12980.8 |   62.6 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |         |        |           |             |
| Sep______ | Floats | 25000 | 14.819 ms |  1.00 | 20 |  1371.2 |  592.8 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  2.042 ms |  0.14 | 20 |  9948.8 |   81.7 | 168.55 KB |       28.80 |
| Sylvan___ | Floats | 25000 | 35.108 ms |  2.37 | 20 |   578.8 | 1404.3 |   18.6 KB |        3.18 |
| CsvHelper | Floats | 25000 | 15.721 ms |  1.06 | 20 |  1292.5 |  628.8 |   6.09 KB |        1.04 |
