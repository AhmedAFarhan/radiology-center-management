#requires -Version 7.0
<#
.SYNOPSIS
Promotes a complete Velopack release into a clinic update share without exposing a partial feed.
.DESCRIPTION
Run as the share administrator. Clients need read access only. Packages are immutable;
the release manifest is replaced last. Existing versions cannot be downgraded or rewritten.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$ReleaseDirectory,
    [Parameter(Mandatory)][string]$Destination,
    [switch]$AllowUnsignedForTesting
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$source = (Resolve-Path -LiteralPath $ReleaseDirectory).Path
if (![IO.Path]::IsPathFullyQualified($Destination)) { throw 'Use an absolute local directory or clinic UNC share.' }
$target = [IO.Path]::GetFullPath($Destination)
if ($source.TrimEnd('\','/') -eq $target.TrimEnd('\','/')) { throw 'Source and destination must be different directories.' }
$manifestPath = Join-Path $source 'releases.win.json'
$manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
$assets = @($manifest.Assets)
if (!$assets.Count) { throw 'The Windows release feed is empty.' }
$names = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
foreach ($asset in $assets) {
    if ($asset.PackageId -ne 'RadiologyCenter' -or $asset.Type -notin @('Full','Delta') -or
        $asset.Version -notmatch '^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$' -or
        $asset.FileName -notmatch '^RadiologyCenter-[0-9]+\.[0-9]+\.[0-9]+-(full|delta)\.nupkg$' -or
        !$names.Add($asset.FileName) -or $asset.SHA256 -notmatch '^[a-fA-F0-9]{64}$') { throw 'Invalid or duplicate release asset.' }
    $expectedName = "RadiologyCenter-$($asset.Version)-$($asset.Type.ToLowerInvariant()).nupkg"
    if ($asset.FileName -cne $expectedName) { throw 'Release asset identity does not match its filename.' }
    $path = Join-Path $source $asset.FileName
    if (!(Test-Path -LiteralPath $path -PathType Leaf) -or (Get-Item -LiteralPath $path).Length -ne [long]$asset.Size -or
        (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash -ne $asset.SHA256) { throw "Missing or corrupt package: $($asset.FileName)" }
}
$full = @($assets | Where-Object Type -eq 'Full' | Sort-Object { [version]$_.Version } -Descending)
if (!$full.Count) { throw 'A complete full package is required.' }
$version = [string]$full[0].Version
if (@($assets | Where-Object { [version]$_.Version -gt [version]$version }).Count) { throw 'A delta update has no corresponding latest full package.' }
if (!$AllowUnsignedForTesting) {
    & (Join-Path $PSScriptRoot 'Assert-ReleaseSignatures.ps1') -ReleaseDirectory $source -Version $version
}
[IO.Directory]::CreateDirectory($target) | Out-Null
$lock = [IO.File]::Open((Join-Path $target '.egcare-publish.lock'), [IO.FileMode]::OpenOrCreate, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
$temporaryFiles = [Collections.Generic.List[string]]::new()
try {
    $targetManifest = Join-Path $target 'releases.win.json'
    if (Test-Path -LiteralPath $targetManifest) {
        $previous = Get-Content -LiteralPath $targetManifest -Raw | ConvertFrom-Json
        $latest = @($previous.Assets | Where-Object Type -eq 'Full' | Sort-Object { [version]$_.Version } -Descending)
        if (!$latest.Count) { throw 'Existing feed is invalid; investigate before replacing it.' }
        if ([version]$version -lt [version]$latest[0].Version) { throw 'Refusing to downgrade the clinic update feed.' }
        if ([version]$version -eq [version]$latest[0].Version -and
            (Get-FileHash -LiteralPath $targetManifest).Hash -ne (Get-FileHash -LiteralPath $manifestPath).Hash) {
            throw 'This version is already present with a different manifest. Publish a new version.'
        }
    }
    foreach ($asset in $assets) {
        $destinationFile = Join-Path $target $asset.FileName
        if (Test-Path -LiteralPath $destinationFile) {
            if ((Get-FileHash -LiteralPath $destinationFile -Algorithm SHA256).Hash -ne $asset.SHA256) { throw 'An existing package has different contents; refusing to overwrite it.' }
            continue
        }
        $temporary = Join-Path $target ('.egcare-incoming-' + [guid]::NewGuid().ToString('N'))
        $temporaryFiles.Add($temporary)
        Copy-Item -LiteralPath (Join-Path $source $asset.FileName) -Destination $temporary
        if ((Get-FileHash -LiteralPath $temporary -Algorithm SHA256).Hash -ne $asset.SHA256) { throw 'Package verification failed after copying to the share.' }
        [IO.File]::Move($temporary, $destinationFile, $false)
    }
    # The stable setup filename is also replaced atomically after its package is available.
    $setup = Join-Path $source 'RadiologyCenter-win-Setup.exe'
    if (Test-Path -LiteralPath $setup) {
        $temporary = Join-Path $target ('.egcare-incoming-' + [guid]::NewGuid().ToString('N'))
        $temporaryFiles.Add($temporary)
        Copy-Item -LiteralPath $setup -Destination $temporary
        if ((Get-FileHash -LiteralPath $setup).Hash -ne (Get-FileHash -LiteralPath $temporary).Hash) { throw 'Installer copy verification failed.' }
        [IO.File]::Move($temporary, (Join-Path $target 'RadiologyCenter-win-Setup.exe'), $true)
    }
    $temporary = Join-Path $target ('.egcare-incoming-' + [guid]::NewGuid().ToString('N'))
    $temporaryFiles.Add($temporary)
    Copy-Item -LiteralPath $manifestPath -Destination $temporary
    if ((Get-FileHash -LiteralPath $manifestPath).Hash -ne (Get-FileHash -LiteralPath $temporary).Hash) { throw 'Manifest copy verification failed.' }
    [IO.File]::Move($temporary, $targetManifest, $true)
    Write-Output "Published EGcare $version to $target. Packages were verified and the feed was switched last."
} finally {
    foreach ($file in $temporaryFiles) { if (Test-Path -LiteralPath $file) { Remove-Item -LiteralPath $file -Force } }
    $lock.Dispose()
}
