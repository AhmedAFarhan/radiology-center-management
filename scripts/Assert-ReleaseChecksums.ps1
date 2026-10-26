#requires -Version 7.0
[CmdletBinding()]
param([Parameter(Mandatory)][string]$ReleaseDirectory)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$root = (Resolve-Path -LiteralPath $ReleaseDirectory).Path
$names = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
foreach ($line in Get-Content -LiteralPath (Join-Path $root 'SHA256SUMS.txt')) {
    if ($line -notmatch '^([a-fA-F0-9]{64})  ([^\\/]+)$') { throw 'Invalid checksum entry.' }
    $hash = $Matches[1]; $name = $Matches[2]
    if ($name -eq 'SHA256SUMS.txt' -or !$names.Add($name)) { throw 'Duplicate or self-referencing checksum entry.' }
    $path = Join-Path $root $name
    if (!(Test-Path -LiteralPath $path -PathType Leaf) -or (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash -ne $hash) { throw "Release file is missing or corrupt: $name" }
}
$files = @(Get-ChildItem -LiteralPath $root -File | Where-Object Name -ne 'SHA256SUMS.txt')
if (!$names.Count -or $files.Count -ne $names.Count -or @($files | Where-Object { !$names.Contains($_.Name) }).Count) { throw 'Every release file must have exactly one checksum.' }
Write-Output 'PASS: all release file SHA-256 checksums match.'
