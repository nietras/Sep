```

BenchmarkDotNet v0.13.10, Ubuntu 22.04.2 LTS (Jammy Jellyfish)
Unknown processor
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  Job-HCCPYN : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  Toolchain=net80  InvocationCount=Default  
IterationTime=350.0000 ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   9.182 ms |  1.00 | 20 | 2207.7 |  367.3 |     1.11 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  30.729 ms |  3.35 | 20 |  659.7 | 1229.1 |     9.74 KB |        8.80 |
| ReadLine_ | Row    | 25000 |  33.696 ms |  3.67 | 20 |  601.6 | 1347.8 | 73489.73 KB |   66,419.67 |
| CsvHelper | Row    | 25000 |  59.282 ms |  6.46 | 20 |  342.0 | 2371.3 |    20.71 KB |       18.71 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |  11.392 ms |  1.00 | 20 | 1779.4 |  455.7 |     1.12 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  34.813 ms |  3.06 | 20 |  582.3 | 1392.5 |     9.75 KB |        8.70 |
| ReadLine_ | Cols   | 25000 |  32.510 ms |  2.85 | 20 |  623.6 | 1300.4 | 73489.75 KB |   65,551.83 |
| CsvHelper | Cols   | 25000 |  64.080 ms |  5.63 | 20 |  316.4 | 2563.2 | 21341.07 KB |   19,035.94 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  52.081 ms |  1.00 | 20 |  389.2 | 2083.2 |     7.97 KB |        1.00 |
| Sylvan___ | Floats | 25000 | 169.677 ms |  3.26 | 20 |  119.5 | 6787.1 |     18.2 KB |        2.28 |
| ReadLine_ | Floats | 25000 | 162.237 ms |  3.11 | 20 |  125.0 | 6489.5 | 73493.46 KB |    9,221.58 |
| CsvHelper | Floats | 25000 | 220.231 ms |  4.23 | 20 |   92.0 | 8809.2 | 22063.34 KB |    2,768.39 |
