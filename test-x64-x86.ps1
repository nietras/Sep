#!/usr/bin/env pwsh
Write-Output "Testing Debug X86"
dotnet test --nologo -c Debug -- RunConfiguration.TargetPlatform=x86 /Parallel
Write-Output "Testing Release X86"
dotnet test --nologo -c Release -- RunConfiguration.TargetPlatform=x86 /Parallel
Write-Output "Testing Debug X64"
dotnet test --nologo -c Debug -- RunConfiguration.TargetPlatform=x64 /Parallel
Write-Output "Testing Release X64"
dotnet test --nologo -c Release --collect:"XPlat Code Coverage" -- RunConfiguration.TargetPlatform=x64 /Parallel