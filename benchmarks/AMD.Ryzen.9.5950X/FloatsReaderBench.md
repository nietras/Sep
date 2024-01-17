```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-XHFWAZ : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Runtime=.NET 8.0  Toolchain=net80  InvocationCount=Default  
IterationTime=350.0000 ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep______ | Floats | 25000 | 22.576 ms |  1.00 | 20 |  900.1 |  903.0 |      8 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  4.115 ms |  0.18 | 20 | 4938.1 |  164.6 | 180.49 KB |       22.55 |
