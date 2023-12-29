```

BenchmarkDotNet v0.13.11, Ubuntu 22.04.2 LTS (Jammy Jellyfish)
Unknown processor
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  Job-HCDRZS : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD

Job=Job-HCDRZS  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350.0000 ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | RatioSD | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|--------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    70.05 ms |  1.00 |    0.00 |  33 |  475.1 | 1400.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    32.78 ms |  0.47 |    0.02 |  33 | 1015.2 |  655.7 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    98.67 ms |  1.41 |    0.06 |  33 |  337.3 | 1973.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    79.84 ms |  1.14 |    0.08 |  33 |  416.8 | 1596.9 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   173.86 ms |  2.48 |    0.09 |  33 |  191.4 | 3477.2 |   13.64 MB |        1.01 |
|           |       |         |             |       |         |     |        |        |            |             |
| Sep______ | Asset | 1000000 | 1,382.02 ms |  1.00 |    0.00 | 665 |  481.8 | 1382.0 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   589.55 ms |  0.43 |    0.00 | 665 | 1129.3 |  589.6 |  260.97 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,962.01 ms |  1.42 |    0.00 | 665 |  339.3 | 1962.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,744.55 ms |  1.26 |    0.01 | 665 |  381.6 | 1744.6 | 2385.08 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 3,454.73 ms |  2.50 |    0.01 | 665 |  192.7 | 3454.7 |  260.58 MB |        1.00 |
