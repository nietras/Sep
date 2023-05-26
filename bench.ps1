dotnet run -c Release -f net7.0 --project src\Sep.Benchmarks\Sep.Benchmarks.csproj -- `
    -m --warmupCount 5 --minIterationCount 3 --maxIterationCount 9 --runtimes net70 `
    --iterationTime 150 --filter *
