#requires -Version 7.0
<#
.SYNOPSIS
Checks a production connection file from a clinic workstation without changing data.
.DESCRIPTION
Run using a Windows identity with SQL administrator access, after admin-PC setup.
Requires Microsoft sqlcmd on PATH. SQL and image certificates must be trusted;
there is deliberately no certificate-bypass option. ImageCredential is optional
only when the profile does not configure an image archive.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$ProfilePath,
    [PSCredential]$ImageCredential
)
$ErrorActionPreference = 'Stop'
$profile = Get-Content -LiteralPath $ProfilePath -Raw | ConvertFrom-Json
$server = [string]$profile.Server; $database = [string]$profile.Database
if ($server -notmatch '^(?:tcp:)?[a-zA-Z0-9][a-zA-Z0-9.-]*(?:\\[a-zA-Z0-9_-]+)?(?:,[0-9]{1,5})?$' -or $database -notmatch '^[a-zA-Z0-9_]{1,100}$') {
    throw 'Invalid clinic server or database name.'
}
$server = $server -replace '^tcp:', ''
$hostName = ($server -split '[\\,]')[0]
if ($server.Contains(',')) {
    $port = [int]($server -split ',')[-1]
    if ($port -lt 1 -or $port -gt 65535) { throw 'Invalid SQL TCP port.' }
}
$addresses = [Net.Dns]::GetHostAddresses($hostName)
if (!$addresses.Count -or @($addresses | Where-Object { [Net.IPAddress]::IsLoopback($_) }).Count) {
    throw 'Use the clinic server network name, not a loopback address.'
}
$sqlcmd = (Get-Command sqlcmd -ErrorAction Stop).Source
$query = @"
SET NOCOUNT ON;
IF NOT EXISTS(SELECT 1 FROM sys.dm_exec_connections WHERE session_id=@@SPID AND net_transport='TCP' AND encrypt_option='TRUE')
    THROW 51000, 'SQL connection must use encrypted TCP.', 1;
IF (SELECT containment FROM sys.databases WHERE name=DB_NAME())<>1
    THROW 51000, 'Clinic database must use partial containment.', 1;
IF NOT EXISTS(SELECT 1 FROM sys.configurations WHERE name='contained database authentication' AND value_in_use=1)
    THROW 51000, 'Contained account authentication is disabled on the clinic server.', 1;
IF OBJECT_ID(N'Security.DesktopSchema',N'U') IS NULL
    THROW 51000, 'Run EGcare admin-PC setup before checking the clinic connection.', 1;
IF NOT EXISTS(SELECT 1 FROM Security.DesktopSchema WHERE Id=1 AND Version=1)
    THROW 51000, 'The database security contract is incompatible with this build.', 1;
IF EXISTS(SELECT 1 FROM [Identity].[AspNetUsers] WHERE UserName LIKE N'demo.%')
    THROW 51000, 'Demo accounts remain in this database. Select the new empty clinic database instead.', 1;
PRINT 'PASS: encrypted SQL TCP, trusted server certificate, contained accounts, clinic security contract and no demo accounts.';
"@
& $sqlcmd -S "tcp:$server" -d $database -E -N -b -l 15 -t 30 -Q $query
if ($LASTEXITCODE -ne 0) { throw 'Clinic SQL verification failed. Check the server TCP listener, firewall, certificate trust and Windows administrator access.' }

if (![string]::IsNullOrWhiteSpace([string]$profile.PacsUrl)) {
    $origin = $null
    if (![Uri]::TryCreate([string]$profile.PacsUrl, [UriKind]::Absolute, [ref]$origin) -or $origin.Scheme -ne 'https' -or $origin.IsLoopback -or $origin.UserInfo -or $origin.AbsolutePath -ne '/' -or $origin.Query -or $origin.Fragment) {
        throw 'PacsUrl must be a non-loopback HTTPS origin without a path or credentials.'
    }
    if (!$ImageCredential) { throw 'Pass -ImageCredential (Get-Credential) to verify the configured archive using a personal image-viewing account.' }
    $base = $origin.GetLeftPart([UriPartial]::Authority)
    $anonymous = Invoke-WebRequest "$base/dicom-web/studies?limit=1" -SkipHttpErrorCheck -MaximumRedirection 0 -TimeoutSec 15
    if ($anonymous.StatusCode -notin @(401,403)) { throw 'The archive did not deny anonymous image access.' }
    $headers = @{ Accept = 'application/dicom+json' }
    $studies = Invoke-WebRequest "$base/dicom-web/studies?limit=1" -Authentication Basic -Credential $ImageCredential -Headers $headers -MaximumRedirection 0 -TimeoutSec 15
    [void]($studies.Content | ConvertFrom-Json)
    $viewer = Invoke-WebRequest "$base/ohif/" -Authentication Basic -Credential $ImageCredential -MaximumRedirection 0 -TimeoutSec 15
    if ($viewer.Content -notmatch '<html' -or $viewer.Headers['X-Frame-Options'] -match 'DENY|SAMEORIGIN') { throw 'OHIF did not return an embeddable viewer.' }
    Write-Output 'PASS: trusted archive HTTPS, authenticated DICOMweb/OHIF and anonymous-access denial.'
} else {
    Write-Output 'Images are not configured in this profile; no archive check was performed.'
}
Write-Output 'Connection checks passed. Complete the two-workstation and real-scanner acceptance walkthrough before clinical use.'
