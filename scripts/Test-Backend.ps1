[CmdletBinding()]
param([string]$ResultsDirectory = 'test-results')
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$failed = @()
$projects = Get-ChildItem -LiteralPath (Join-Path $root 'src/Backend/Modules') -Filter '*.IntegrationTests.csproj' -Recurse | Sort-Object FullName
foreach ($project in $projects) {
    dotnet test $project.FullName --configuration Release --logger "trx;LogFileName=$($project.BaseName).trx" --results-directory $ResultsDirectory
    if ($LASTEXITCODE -ne 0) { $failed += $project.BaseName }
}
if ($failed.Count) { throw "Integration suites failed: $($failed -join ', ')" }
