param(
    [string]$filter = "*",
    [string]$tfm = "net11.0"
)
dotnet run -c Release -f ${tfm} --project src\Sep.Benchmarks\Sep.Benchmarks.csproj -- -m --warmupCount 5 --minIterationCount 3 --maxIterationCount 9 --runtimes ${tfm} --iterationTime 300 --filter "$filter"
