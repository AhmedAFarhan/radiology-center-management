#requires -Version 7.0
[CmdletBinding()]
param([string]$OrthancDirectory = 'C:\Program Files\Orthanc Server')
$ErrorActionPreference = 'Stop'
$binary = Join-Path $OrthancDirectory 'Orthanc.exe'
$plugins = @('OrthancDicomWeb.dll', 'libOrthancOHIF-Windows64.dll') | ForEach-Object { Join-Path $OrthancDirectory "Plugins\$_" }
foreach ($file in @($binary) + $plugins) { if (!(Test-Path -LiteralPath $file)) { throw "Missing installed archive component: $file" } }
# This probe never uses the installed service's configuration, archive or DICOM port.
$directory = Join-Path ([IO.Path]::GetTempPath()) ('EGcareArchiveTest-' + [guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory($directory) | Out-Null
$listener = [Net.Sockets.TcpListener]::new([Net.IPAddress]::Loopback, 0)
$listener.Start(); $port = $listener.LocalEndpoint.Port; $listener.Stop()
$adminPassword = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
$viewerPassword = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
$key = [Security.Cryptography.RSA]::Create(2048)
$request = [Security.Cryptography.X509Certificates.CertificateRequest]::new('CN=localhost', $key, [Security.Cryptography.HashAlgorithmName]::SHA256, [Security.Cryptography.RSASignaturePadding]::Pkcs1)
$certificate = $request.CreateSelfSigned([DateTimeOffset]::UtcNow.AddMinutes(-1), [DateTimeOffset]::UtcNow.AddDays(1))
$pem = Join-Path $directory 'test-only.pem'
[IO.File]::WriteAllText($pem, $certificate.ExportCertificatePem() + "`n" + $key.ExportPkcs8PrivateKeyPem())
$config = @{
    Name = 'EGcare isolated synthetic image test'; StorageDirectory = (Join-Path $directory 'archive'); IndexDirectory = (Join-Path $directory 'archive')
    DicomServerEnabled = $false; HttpPort = $port; RemoteAccessAllowed = $false; AuthenticationEnabled = $true
    SslEnabled = $true; SslCertificate = $pem; RegisteredUsers = @{ 'archive-admin' = $adminPassword; 'test-viewer' = $viewerPassword }
    Plugins = $plugins; LuaScripts = @((Join-Path $PSScriptRoot 'OrthancReadOnly.lua'))
    ExecuteLuaEnabled = $false; RestApiWriteToFileSystemEnabled = $false; WebDavEnabled = $false
    DicomWeb = @{ Enable = $true; Root = '/dicom-web/'; PublicRoot = '/dicom-web/'; Ssl = $true }
    OHIF = @{ DataSource = 'dicom-web'; RouterBasename = '/ohif/' }
}
$configPath = Join-Path $directory 'orthanc-test.json'
$config | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $configPath -Encoding utf8
$process = [Diagnostics.Process]::new()
$process.StartInfo = [Diagnostics.ProcessStartInfo]::new($binary)
$process.StartInfo.ArgumentList.Add($configPath)
$process.StartInfo.UseShellExecute = $false; $process.StartInfo.CreateNoWindow = $true
$process.StartInfo.RedirectStandardOutput = $true; $process.StartInfo.RedirectStandardError = $true
$base = "https://127.0.0.1:$port"
$started = $false
function Header([string]$user, [string]$password) { @{ Authorization = 'Basic ' + [Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($user + ':' + $password)) } }
try {
    [void]$process.Start()
    $started = $true
    $stdout = $process.StandardOutput.ReadToEndAsync(); $stderr = $process.StandardError.ReadToEndAsync()
    $adminHeaders = Header 'archive-admin' $adminPassword; $viewerHeaders = Header 'test-viewer' $viewerPassword
    $ready = $false
    for ($attempt = 0; $attempt -lt 60; $attempt++) {
        if ($process.HasExited) { throw 'The isolated archive process exited during startup.' }
        try {
            Invoke-WebRequest "$base/dicom-web/studies" -Headers $viewerHeaders -SkipCertificateCheck -TimeoutSec 2 | Out-Null
            $ready = $true; break
        } catch { Start-Sleep -Milliseconds 250 }
    }
    if (!$ready) { throw 'The isolated archive did not become ready.' }
    # Certificate checking is bypassed only for this isolated, disposable self-signed test service.
    $patient = 'P-TEST-' + [guid]::NewGuid().ToString('N'); $accession = 'EX-TEST-' + [guid]::NewGuid().ToString('N')
    $imagePath = Join-Path (Split-Path $PSScriptRoot -Parent) 'src/Desktop/RadiologyCenter.Desktop/wwwroot/images/healthcare-logo-64.png'
    $payload = @{ Tags = @{ PatientID = $patient; PatientName = 'SYNTHETIC^TEST'; AccessionNumber = $accession; Modality = 'OT' }; Content = 'data:image/png;base64,' + [Convert]::ToBase64String([IO.File]::ReadAllBytes($imagePath)) } | ConvertTo-Json -Depth 5
    $created = Invoke-RestMethod "$base/tools/create-dicom" -Method Post -Headers $adminHeaders -ContentType 'application/json' -Body $payload -SkipCertificateCheck
    $studies = @(Invoke-RestMethod "$base/dicom-web/studies?PatientID=$patient" -Headers $viewerHeaders -SkipCertificateCheck)
    if ($studies.Count -ne 1 -or $studies[0].'00100020'.Value[0] -ne $patient -or $studies[0].'00080050'.Value[0] -ne $accession) { throw 'DICOMweb returned an incorrect synthetic study identity.' }
    $series = @(Invoke-RestMethod "$base/dicom-web/studies/$($studies[0].'0020000D'.Value[0])/series" -Headers $viewerHeaders -SkipCertificateCheck)
    if ($series.Count -ne 1) { throw 'The synthetic image series is missing.' }
    $studyUid = $studies[0].'0020000D'.Value[0]; $seriesUid = $series[0].'0020000E'.Value[0]
    $instances = @(Invoke-RestMethod "$base/dicom-web/studies/$studyUid/series/$seriesUid/instances" -Headers $viewerHeaders -SkipCertificateCheck)
    if ($instances.Count -ne 1) { throw 'The synthetic image instance is missing.' }
    $instanceUid = $instances[0].'00080018'.Value[0]
    $frameHeaders = $viewerHeaders.Clone(); $frameHeaders.Accept = 'multipart/related; type="application/octet-stream"'
    $frame = Invoke-WebRequest "$base/dicom-web/studies/$studyUid/series/$seriesUid/instances/$instanceUid/frames/1" -Headers $frameHeaders -SkipCertificateCheck
    if ($frame.StatusCode -ne 200 -or $frame.RawContentLength -le 0) { throw 'The synthetic image frame could not be retrieved.' }
    $viewer = Invoke-WebRequest "$base/ohif/" -Headers $viewerHeaders -SkipCertificateCheck
    if ($viewer.StatusCode -ne 200 -or $viewer.Content -notmatch '<html') { throw 'OHIF did not return its viewer document.' }
    if ($viewer.Headers['X-Frame-Options'] -match 'DENY|SAMEORIGIN') { throw 'The image service forbids the embedded desktop viewer.' }
    $deleted = Invoke-WebRequest "$base/instances/$($created.ID)" -Method Delete -Headers $viewerHeaders -SkipCertificateCheck -SkipHttpErrorCheck
    if ($deleted.StatusCode -notin @(401,403)) { throw 'An image-viewing account was allowed to modify the archive.' }
    $anonymous = Invoke-WebRequest "$base/dicom-web/studies" -SkipCertificateCheck -SkipHttpErrorCheck
    if ($anonymous.StatusCode -notin @(401,403)) { throw 'Anonymous image access was allowed.' }
    Write-Output 'PASS: installed DICOMweb/OHIF plugins, HTTPS endpoint, exact synthetic patient/accession, series/instance/frame retrieval, authenticated viewer, anonymous denial and read-only image permissions.'
    # A consistent SQLite/index + object backup requires the archive to be stopped.
    # Verify a restored copy independently of the original synthetic archive.
    $originalFile = Invoke-WebRequest "$base/instances/$($created.ID)/file" -Headers $adminHeaders -SkipCertificateCheck
    $originalHash = [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$originalFile.Content))
    $process.Kill(); $process.WaitForExit(); $started = $false
    [IO.File]::WriteAllText((Join-Path $directory 'original-stdout.log'), $stdout.GetAwaiter().GetResult())
    [IO.File]::WriteAllText((Join-Path $directory 'original-stderr.log'), $stderr.GetAwaiter().GetResult())
    $process.Dispose()
    $restored = Join-Path $directory 'restored-archive'
    Copy-Item -LiteralPath $config.StorageDirectory -Destination $restored -Recurse
    $config.StorageDirectory = $restored; $config.IndexDirectory = $restored
    $config | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $configPath -Encoding utf8
    $process = [Diagnostics.Process]::new()
    $process.StartInfo = [Diagnostics.ProcessStartInfo]::new($binary)
    $process.StartInfo.ArgumentList.Add($configPath)
    $process.StartInfo.UseShellExecute = $false; $process.StartInfo.CreateNoWindow = $true
    $process.StartInfo.RedirectStandardOutput = $true; $process.StartInfo.RedirectStandardError = $true
    [void]$process.Start()
    $started = $true
    $stdout = $process.StandardOutput.ReadToEndAsync(); $stderr = $process.StandardError.ReadToEndAsync()
    $ready = $false
    for ($attempt = 0; $attempt -lt 60; $attempt++) {
        if ($process.HasExited) { throw 'The restored archive process exited during startup.' }
        try {
            $restoredStudies = @(Invoke-RestMethod "$base/dicom-web/studies?PatientID=$patient" -Headers $viewerHeaders -SkipCertificateCheck -TimeoutSec 2)
            $ready = $true; break
        } catch { Start-Sleep -Milliseconds 250 }
    }
    if (!$ready -or $restoredStudies.Count -ne 1 -or $restoredStudies[0].'0020000D'.Value[0] -ne $studyUid -or $restoredStudies[0].'00080050'.Value[0] -ne $accession) { throw 'The restored archive did not preserve the study identity.' }
    $restoredFrame = Invoke-WebRequest "$base/dicom-web/studies/$studyUid/series/$seriesUid/instances/$instanceUid/frames/1" -Headers $frameHeaders -SkipCertificateCheck
    # Compare the exact stored DICOM file; multipart frame boundaries can change per response.
    $restoredFile = Invoke-WebRequest "$base/instances/$($created.ID)/file" -Headers $adminHeaders -SkipCertificateCheck
    $restoredHash = [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$restoredFile.Content))
    if ($restoredFrame.RawContentLength -le 0 -or $originalHash -ne $restoredHash) { throw 'Restored image content differs from the original.' }
    Write-Output 'PASS: stopped-archive backup restored into separate storage; study identity, exact DICOM file hash and image retrieval survived recovery.'
} finally {
    if ($started -and !$process.HasExited) { $process.Kill(); $process.WaitForExit() }
    if ($stdout) { [IO.File]::WriteAllText((Join-Path $directory 'stdout.log'), $stdout.GetAwaiter().GetResult()) }
    if ($stderr) { [IO.File]::WriteAllText((Join-Path $directory 'stderr.log'), $stderr.GetAwaiter().GetResult()) }
    $process.Dispose(); $certificate.Dispose(); $key.Dispose()
    Write-Output "Isolated synthetic evidence retained at $directory"
}
