param(
    [string]$filter = "*"
)
dotnet run -c Release -f net11.0 --project src\Sep.Benchmarks\Sep.Benchmarks.csproj -- -m --warmupCount 5 --minIterationCount 3 --maxIterationCount 9 --runtimes net11.0 --iterationTime 300 --filter "$filter"
