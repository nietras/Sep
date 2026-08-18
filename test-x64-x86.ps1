#!/usr/bin/env pwsh
Write-Output "Testing Debug X86"
dotnet test -c Debug --arch x86
Write-Output "Testing Release X86"
dotnet test -c Release --arch x86
Write-Output "Testing Debug X64"
dotnet test -c Debug --arch x64
Write-Output "Testing Release X64"
dotnet test -c Release --arch x64 --coverage --coverage-output-format cobertura
