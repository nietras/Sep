param(
    [string]$runtime = "win-x64",
    [string]$tfm = "net11.0"
)
dotnet publish -r ${runtime} -f ${tfm} -c Release -p:PublishAot=true .\src\Sep.tester\Sep.Tester.csproj
dir "artifacts\publish\Sep.Tester\release_${tfm}_${runtime}\Sep.Tester.exe"
