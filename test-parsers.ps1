#!/usr/bin/env pwsh
$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

try {
    dotnet build .\src\Sep.Test\Sep.Test.csproj --nologo -c Debug --no-restore
    if ($LASTEXITCODE -ne 0) {
        throw "Debug build failed with exit code $LASTEXITCODE."
    }
    dotnet build .\src\Sep.Test\Sep.Test.csproj --nologo -c Release --no-restore
    if ($LASTEXITCODE -ne 0) {
        throw "Release build failed with exit code $LASTEXITCODE."
    }

    $parsers = @(
        "SepParserAdvSimdLoad4xNrwCmpOrBulkMoveMaskTzcnt",
        "SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt",
        "SepParserAvx512To256CmpOrMoveMaskTzcnt",
        "SepParserAvx512PackCmpOrMoveMaskTzcnt",
        "SepParserAvx2PackCmpOrMoveMaskTzcnt",
        "SepParserAvx256To128CmpOrMoveMaskTzcnt",
        "SepParserSse2PackCmpOrMoveMaskTzcnt",
        "SepParserVector512NrwCmpExtMsbTzcnt",
        "SepParserVector256NrwCmpExtMsbTzcnt",
        "SepParserVector128NrwCmpExtMsbTzcnt",
        "SepParserVector64NrwCmpExtMsbTzcnt",
        "SepParserIndexOfAny"
    )

    foreach ($parser in $parsers) {
        $env:SEPFORCEPARSER = $parser
        Write-Output "Testing $parser Debug"
        dotnet test --project .\src\Sep.Test\Sep.Test.csproj -c Debug --no-build --no-restore
        if ($LASTEXITCODE -ne 0) {
            throw "Debug parser test failed for $parser with exit code $LASTEXITCODE."
        }
        Write-Output "Testing $parser Release"
        dotnet test --project .\src\Sep.Test\Sep.Test.csproj -c Release --no-build --no-restore
        if ($LASTEXITCODE -ne 0) {
            throw "Release parser test failed for $parser with exit code $LASTEXITCODE."
        }
    }
} finally {
    Remove-Item env:SEPFORCEPARSER -ErrorAction SilentlyContinue
}
