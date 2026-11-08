param(
    [switch]$all,
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$dotnetArgs
)

if ($all) {
    dotnet build -p:BuildAllTargetFrameworks=true @dotnetArgs
} else {
    dotnet build @dotnetArgs
}
