#requires -Version 7.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'ReleaseVersion.ps1')
function Read-Fixture([string]$Version, [string]$Build) {
    ConvertFrom-ReleaseVersionXml "<Project><PropertyGroup><EgcareReleaseVersion>$Version</EgcareReleaseVersion><EgcareReleaseBuild>$Build</EgcareReleaseBuild></PropertyGroup></Project>"
}
function Expect-Failure([scriptblock]$Action, [string]$Name) {
    $failed = $false
    try { & $Action | Out-Null } catch { $failed = $true }
    if (!$failed) { throw "Expected rejection: $Name" }
    Write-Output "PASS: $Name"
}
$first = Read-Fixture '1.0.0' '1'
$next = Read-Fixture '1.0.1' '2'
Assert-ReleaseTransition $first $null
Assert-ReleaseTransition $next $first
Assert-ReleaseInputs $first '1.0.0' '0'
Assert-ReleaseInputs $first '1.0.0' '1'
Write-Output 'PASS: first release, increasing release and matching manual inputs'
Expect-Failure { Assert-ReleaseTransition $first $first } 'same version rejected'
Expect-Failure { Assert-ReleaseTransition $first $next } 'downgrade rejected'
Expect-Failure { Assert-ReleaseTransition (Read-Fixture '1.0.2' '1') $first } 'reused build rejected'
Expect-Failure { Assert-ReleaseTransition $next $null } 'unexpected first release rejected'
Expect-Failure { Assert-ReleaseInputs $first '1.0.1' '1' } 'unreviewed version rejected'
Expect-Failure { Assert-ReleaseInputs $first '1.0.0' '2' } 'unreviewed build rejected'
foreach ($version in @('01.0.0','1.0.0-beta','1.0','65535.0.0')) {
    Expect-Failure { Read-Fixture $version '1' } "invalid version $version rejected"
}
foreach ($build in @('0','01','65536','1.5')) {
    Expect-Failure { Read-Fixture '1.0.0' $build } "invalid build $build rejected"
}
Expect-Failure { ConvertFrom-ReleaseVersionXml '<!DOCTYPE Project [<!ENTITY example "1.0.0">]><Project />' } 'DTD rejected'
