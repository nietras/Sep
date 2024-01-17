```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-LJHOJV : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-LJHOJV  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350.0000 ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean     | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |---------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 1000000 | 450.8 ms |  1.00 | 583 | 1295.1 |  450.8 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 135.2 ms |  0.30 | 583 | 4317.5 |  135.2 | 261.45 MB |        1.00 |
