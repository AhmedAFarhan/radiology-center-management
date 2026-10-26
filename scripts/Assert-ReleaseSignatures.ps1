#requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$ReleaseDirectory,
    [Parameter(Mandatory)][ValidatePattern('^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$')][string]$Version
)
$ErrorActionPreference = 'Stop'
$setup = Join-Path $ReleaseDirectory 'RadiologyCenter-win-Setup.exe'
$setupSignature = Get-AuthenticodeSignature -LiteralPath $setup
if ($setupSignature.Status -ne 'Valid') { throw 'Installer signature is missing or invalid.' }
$feed = Get-Content -LiteralPath (Join-Path $ReleaseDirectory 'releases.win.json') -Raw | ConvertFrom-Json
$assets = @($feed.Assets | Where-Object { $_.Version -eq $Version -and $_.Type -eq 'Full' })
if ($assets.Count -ne 1 -or [IO.Path]::GetFileName($assets[0].FileName) -ne $assets[0].FileName) { throw 'The selected full release is missing or ambiguous.' }
$package = [IO.Compression.ZipFile]::OpenRead((Join-Path $ReleaseDirectory $assets[0].FileName))
$probe = Join-Path ([IO.Path]::GetTempPath()) ('EGcare-signature-' + [guid]::NewGuid().ToString('N') + '.exe')
try {
    $entry = @($package.Entries | Where-Object { $_.FullName.EndsWith('/RadiologyCenter.Desktop.exe') })
    if ($entry.Count -ne 1) { throw 'Packaged application executable is missing or ambiguous.' }
    [IO.Compression.ZipFileExtensions]::ExtractToFile($entry[0], $probe)
    $appSignature = Get-AuthenticodeSignature -LiteralPath $probe
    if ($appSignature.Status -ne 'Valid') { throw 'Packaged application signature is missing or invalid.' }
    if ($appSignature.SignerCertificate.Thumbprint -ne $setupSignature.SignerCertificate.Thumbprint) { throw 'Installer and application must have the same signing certificate.' }
} finally {
    $package.Dispose()
    if (Test-Path -LiteralPath $probe) { Remove-Item -LiteralPath $probe }
}
Write-Output 'PASS: installer and packaged application signatures are valid and match.'
