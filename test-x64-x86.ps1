#!/usr/bin/env pwsh
$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

$testProjects = @(
    ".\src\Sep.Test\Sep.Test.csproj"
    ".\src\Sep.XyzTest\Sep.XyzTest.csproj"
)
$configurations = @(
    "Debug"
    "Release"
)
$architectures = @(
    "x86"
    "x64"
)

foreach ($architecture in $architectures) {
    foreach ($configuration in $configurations) {
        Write-Output "Testing $configuration $($architecture.ToUpperInvariant())"

        foreach ($project in $testProjects) {
            dotnet test --project $project --nologo -c $configuration --arch $architecture
        }
    }
}
