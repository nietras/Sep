#!/usr/bin/env pwsh
Try {
    $parsers = @( 
        "SepParserAvx512PackCmpOrMoveMaskTzcnt", 
        "SepParserAvx2PackCmpOrMoveMaskTzcnt", 
        "SepParserSse2PackCmpOrMoveMaskTzcnt", 
        "SepParserVector512NrwCmpExtMsbTzcnt", 
        "SepParserVector256NrwCmpExtMsbTzcnt", 
        "SepParserVector128NrwCmpExtMsbTzcnt", 
        "SepParserVector64NrwCmpExtMsbTzcnt", 
        "SepParserIndexOfAny" 
    )

    foreach ($parser in $parsers) {
        $env:SEPFORCEPARSER=$parser
        Write-Host "Testing $parser Debug"
        dotnet test --nologo -c Debug -- /Parallel
        Write-Host "Testing $parser Release"
        dotnet test --nologo -c Release -- /Parallel 
        #Write-Host "Testing $parser Debug X86"
        #dotnet test --nologo -c Debug -- RunConfiguration.TargetPlatform=x86 /Parallel
        #Write-Host "Testing $parser Release X86"
        #dotnet test --nologo -c Release -- RunConfiguration.TargetPlatform=x86 /Parallel
        #Write-Host "Testing $parser Debug X64"
        #dotnet test --nologo -c Debug -- RunConfiguration.TargetPlatform=x64 /Parallel
        #Write-Host "Testing $parser Release X64"
        #dotnet test --nologo -c Release --collect:"XPlat Code Coverage" -- RunConfiguration.TargetPlatform=x64 /Parallel 
    }
} Finally {
    Remove-item env:SEPFORCEPARSER
}