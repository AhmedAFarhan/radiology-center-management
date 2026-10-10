#requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Server,
    [string]$Database = 'EGcareClinic',
    [string]$PacsUrl,
    [string]$UpdatesDirectory,
    [string]$OutputPath = 'EGcare-clinic.json'
)
$ErrorActionPreference = 'Stop'
if ($Server -notmatch '^(?:tcp:)?[a-zA-Z0-9][a-zA-Z0-9.-]*(?:\\[a-zA-Z0-9_-]+)?(?:,[0-9]{1,5})?$') {
    throw 'Use the admin PC network name or IPv4 address, with an optional instance or port.'
}
$hostName = (($Server -replace '^tcp:', '') -split '[,\\]')[0]
if ($hostName -in @('localhost', '127.0.0.1', '(local)', '(localdb)', '.')) {
    throw 'Use the admin PC network name so every workstation reaches the same database.'
}
if ($Server.Contains(',')) {
    $port = [int]($Server -split ',')[-1]
    if ($port -lt 1 -or $port -gt 65535) { throw 'The TCP port must be between 1 and 65535.' }
}
if ($Database -notmatch '^[a-zA-Z0-9_]{1,100}$') { throw 'Use letters, numbers and underscores for the database name.' }
$profile = [ordered]@{ Server = $Server; Database = $Database }
if ($PacsUrl) {
    $address = [uri]$PacsUrl
    if (!$address.IsAbsoluteUri -or $address.Scheme -ne 'https' -or $address.IsLoopback -or $address.UserInfo -or $address.AbsolutePath -ne '/' -or $address.Query -or $address.Fragment) {
        throw 'PacsUrl must be an HTTPS origin, such as https://ADMIN-PC.clinic.test:8042.'
    }
    $profile.PacsUrl = $address.GetLeftPart('Authority')
}
if ($UpdatesDirectory) {
    if (![IO.Path]::IsPathFullyQualified($UpdatesDirectory)) { throw 'Use an absolute local path or UNC share for updates.' }
    $profile.UpdatesDirectory = $UpdatesDirectory
}
$path = [IO.Path]::GetFullPath($OutputPath)
if (Test-Path -LiteralPath $path) { throw 'The connection file already exists. Choose a new output filename.' }
[IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($path)) | Out-Null
$profile | ConvertTo-Json | Set-Content -LiteralPath $path -Encoding utf8
Write-Output "Created public clinic connection file: $path. Import it in EGcare on each workstation. It contains no passwords."
