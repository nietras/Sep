#!/usr/bin/env pwsh
$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

$testProjects = @(
    ".\src\Sep.Test\Sep.Test.csproj"
    ".\src\Sep.XyzTest\Sep.XyzTest.csproj"
)

function Test-Projects {
    param(
        [string]$Configuration,
        [string]$Architecture,
        [switch]$Coverage
    )

    foreach ($project in $testProjects) {
        $arguments = @(
            "--project", $project,
            "--nologo",
            "-c", $Configuration,
            "--arch", $Architecture
        )
        if ($Coverage) {
            $arguments += "--coverage", "--coverage-output-format", "cobertura"
        }
        dotnet test @arguments
        if ($LASTEXITCODE -ne 0) {
            throw "dotnet test failed for $project ($Configuration, $Architecture) with exit code $LASTEXITCODE."
        }
    }
}

Write-Output "Testing Debug X86"
Test-Projects Debug x86
Write-Output "Testing Release X86"
Test-Projects Release x86
Write-Output "Testing Debug X64"
Test-Projects Debug x64
Write-Output "Testing Release X64"
Test-Projects Release x64 -Coverage
