#requires -Version 7.0
[CmdletBinding()]
param([Parameter(Mandatory)][string]$AppDirectory)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path -LiteralPath $AppDirectory).Path
if (!(Test-Path -LiteralPath (Join-Path $root 'RadiologyCenter.Desktop.exe'))) { throw 'Published desktop executable is missing.' }
$forbidden = Get-ChildItem -LiteralPath $root -Recurse -Force | Where-Object {
    $_.Name -in @('appsettings.Development.json','clinic.json','client.json','server.json') -or
    $_.Name -like 'RadiologyCenter.Localhost*' -or
    (!$_.PSIsContainer -and $_.Extension -in @('.pfx','.pem','.bak','.mdf','.ldf')) -or
    ($_.PSIsContainer -and $_.Name -like '*WebView2*')
}
if ($forbidden) { throw "Private configuration, data, signing material or legacy files must not be packaged: $($forbidden.Name -join ', ')" }
Write-Output 'PASS: single application payload; no development configuration, database files or signing secrets.'
