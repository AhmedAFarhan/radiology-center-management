#requires -Version 7.0
[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$temporaryRoot = [IO.Path]::GetFullPath([IO.Path]::GetTempPath())
$root = Join-Path $temporaryRoot ('EGcareReleaseTests-' + [guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory($root) | Out-Null
function New-TestFeed([string]$Version) {
    $path = Join-Path $root $Version
    [IO.Directory]::CreateDirectory($path) | Out-Null
    $name = "RadiologyCenter-$Version-full.nupkg"
    $package = Join-Path $path $name
    [IO.File]::WriteAllText($package, "Synthetic package fixture $Version")
    $asset = @{ PackageId='RadiologyCenter'; Version=$Version; Type='Full'; FileName=$name; SHA256=(Get-FileHash -LiteralPath $package).Hash; Size=(Get-Item -LiteralPath $package).Length }
    @{ Assets=@($asset) } | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $path 'releases.win.json')
    return $path
}
function Expect-Failure([scriptblock]$Action, [string]$Description) {
    $failed = $false
    try { & $Action | Out-Null } catch { $failed = $true }
    if (!$failed) { throw "Expected failure: $Description" }
    Write-Output "PASS: $Description"
}
try {
    $destination = Join-Path $root 'share'
    $first = New-TestFeed '1.0.0'
    $second = New-TestFeed '1.0.1'
    & (Join-Path $PSScriptRoot 'Publish-ClinicUpdateFeed.ps1') -ReleaseDirectory $first -Destination $destination | Out-Null
    & (Join-Path $PSScriptRoot 'Publish-ClinicUpdateFeed.ps1') -ReleaseDirectory $second -Destination $destination | Out-Null
    & (Join-Path $PSScriptRoot 'Publish-ClinicUpdateFeed.ps1') -ReleaseDirectory $second -Destination $destination | Out-Null
    $manifest = Join-Path $destination 'releases.win.json'
    $originalHash = (Get-FileHash -LiteralPath $manifest).Hash
    if ((Get-Content -LiteralPath $manifest -Raw | ConvertFrom-Json).Assets[0].Version -ne '1.0.1') { throw 'Latest feed was not promoted.' }
    if (!(Test-Path -LiteralPath (Join-Path $destination 'RadiologyCenter-1.0.0-full.nupkg'))) { throw 'Earlier package was removed while clients could still be downloading it.' }
    Write-Output 'PASS: unsigned release promotion, idempotent retry and earlier-package retention.'
    Expect-Failure { & (Join-Path $PSScriptRoot 'Publish-ClinicUpdateFeed.ps1') -ReleaseDirectory $first -Destination $destination } 'downgrade rejected'
    $third = New-TestFeed '1.0.2'
    Add-Content -LiteralPath (Join-Path $third 'RadiologyCenter-1.0.2-full.nupkg') -Value 'corruption'
    Expect-Failure { & (Join-Path $PSScriptRoot 'Publish-ClinicUpdateFeed.ps1') -ReleaseDirectory $third -Destination $destination } 'corrupt package rejected'
    $bad = Get-Content -LiteralPath (Join-Path $third 'releases.win.json') -Raw | ConvertFrom-Json
    $bad.Assets[0].FileName = '../escape.nupkg'
    $bad | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $third 'releases.win.json')
    Expect-Failure { & (Join-Path $PSScriptRoot 'Publish-ClinicUpdateFeed.ps1') -ReleaseDirectory $third -Destination $destination } 'path traversal rejected'
    Expect-Failure { & (Join-Path $PSScriptRoot 'Publish-ClinicUpdateFeed.ps1') -ReleaseDirectory $second -Destination $destination -RequireSignature } 'explicit signature requirement rejects an unsigned release'
    if ((Get-FileHash -LiteralPath $manifest).Hash -ne $originalHash) { throw 'A rejected promotion changed the live manifest.' }
    Write-Output 'PASS: rejected promotions preserve the current feed.'
    $checksum = Join-Path $second 'SHA256SUMS.txt'
    Get-ChildItem -LiteralPath $second -File | ForEach-Object { "$((Get-FileHash -LiteralPath $_.FullName).Hash)  $($_.Name)" } | Set-Content -LiteralPath $checksum
    & (Join-Path $PSScriptRoot 'Assert-ReleaseChecksums.ps1') -ReleaseDirectory $second
    Add-Content -LiteralPath (Join-Path $second 'RadiologyCenter-1.0.1-full.nupkg') -Value 'corruption'
    Expect-Failure { & (Join-Path $PSScriptRoot 'Assert-ReleaseChecksums.ps1') -ReleaseDirectory $second } 'release checksum rejects changed files'
    [IO.File]::WriteAllText($checksum, ('a' * 64) + '  ../escape.nupkg')
    Expect-Failure { & (Join-Path $PSScriptRoot 'Assert-ReleaseChecksums.ps1') -ReleaseDirectory $second } 'release checksum rejects path traversal'
    $app = Join-Path $root 'app'; [IO.Directory]::CreateDirectory($app) | Out-Null
    [IO.File]::WriteAllText((Join-Path $app 'RadiologyCenter.Desktop.exe'), 'payload fixture')
    [IO.File]::WriteAllText((Join-Path $app 'clinic.json'), '{}')
    Expect-Failure { & (Join-Path $PSScriptRoot 'Assert-ReleasePayload.ps1') -AppDirectory $app } 'private clinic configuration rejected from package'
} finally {
    $resolved = [IO.Path]::GetFullPath($root)
    if (!$resolved.StartsWith($temporaryRoot, [StringComparison]::OrdinalIgnoreCase) -or [IO.Path]::GetFileName($resolved) -notlike 'EGcareReleaseTests-*') { throw 'Refusing cleanup outside the generated temporary test directory.' }
    Remove-Item -LiteralPath $resolved -Recurse -Force
}
