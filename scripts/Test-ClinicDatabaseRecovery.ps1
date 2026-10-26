#requires -Version 7.0
<#
.SYNOPSIS
Creates a checksum-protected copy-only backup and restores it into a disposable database.
.DESCRIPTION
Requires Microsoft's sqlcmd and a Windows SQL administrator account. BackupDirectory
is on SourceServer; RestoreDirectory is on RecoveryServer. The recovery SQL service
must be able to read the backup path. Existing databases/files are never overwritten.
The backup is retained; only the randomly named recovery database is dropped.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$SourceServer,
    [Parameter(Mandatory)][ValidatePattern('^[A-Za-z0-9_]{1,100}$')][string]$Database,
    [string]$BackupDirectory,
    [string]$RecoveryServer = '(localdb)\EGcareIntegrationTests',
    [string]$RestoreDirectory,
    [switch]$TrustLocalSourceCertificate,
    [switch]$TrustLocalRecoveryCertificate
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$sqlcmdCommand = Get-Command sqlcmd -ErrorAction SilentlyContinue
if (!$sqlcmdCommand) { throw 'Install Microsoft sqlcmd and add it to PATH before running this drill.' }
$sqlcmd = $sqlcmdCommand.Source

function Sql-Literal([string]$Value) { return "N'" + $Value.Replace("'", "''") + "'" }
function Sql-Identifier([string]$Value) { return '[' + $Value.Replace(']', ']]') + ']' }
function Invoke-Sql([string]$Server, [bool]$TrustLocal, [string]$Sql, [switch]$Json) {
    $hostName = ($Server -split '[\\,]')[0]
    $local = $hostName -in @('.', '(local)', '(localdb)', 'localhost', '127.0.0.1', '::1', $env:COMPUTERNAME)
    if ($TrustLocal -and !$local) { throw 'Certificate bypass is allowed only for a local development/test SQL instance.' }
    $file = [IO.Path]::GetTempFileName()
    try {
        [IO.File]::WriteAllText($file, "SET NOCOUNT ON;`n$Sql", [Text.UTF8Encoding]::new($false))
        $arguments = @('-S', $Server, '-d', 'master', '-E', '-b', '-r', '1', '-h', '-1', '-w', '65535', '-y', '8000', '-f', '65001', '-l', '15', '-t', '1800', '-i', $file)
        # LocalDB is a Windows-user-local named pipe and does not support TLS via sqlcmd.
        # Every ordinary SQL instance, including a local full instance, requires encryption.
        if ($hostName -ne '(localdb)') { $arguments += '-N' }
        if ($TrustLocal) { $arguments += '-C' }
        $output = & $sqlcmd @arguments 2>&1
        if ($LASTEXITCODE -ne 0) { throw "SQL recovery operation failed: $($output -join [Environment]::NewLine)" }
        if ($Json) { return (($output | ForEach-Object { $_.ToString().TrimEnd() }) -join '' | ConvertFrom-Json) }
    } finally { Remove-Item -LiteralPath $file -Force }
}

$restoreAttempted = $false
$recoveryName = 'EGcare_Recovery_' + [guid]::NewGuid().ToString('N')
$recoveryId = Sql-Identifier $recoveryName
$sourceTrust = $TrustLocalSourceCertificate.IsPresent
$recoveryTrust = $TrustLocalRecoveryCertificate.IsPresent
try {
    $sourceId = Sql-Identifier $Database; $databaseLiteral = Sql-Literal $Database
    $sourceInfo = @(Invoke-Sql $SourceServer $sourceTrust "SELECT name, state_desc FROM sys.databases WHERE name=$databaseLiteral AND database_id>4 FOR JSON PATH" -Json)
    if ($sourceInfo.Count -ne 1 -or $sourceInfo[0].state_desc -ne 'ONLINE') { throw 'The source must be an existing, online user database.' }
    if (!$BackupDirectory) { $BackupDirectory = (Invoke-Sql $SourceServer $sourceTrust "SELECT CAST(SERVERPROPERTY('InstanceDefaultBackupPath') AS nvarchar(4000)) AS Path FOR JSON PATH" -Json).Path }
    if (![IO.Path]::IsPathFullyQualified($BackupDirectory)) { throw 'Specify an absolute backup directory on the source SQL server.' }
    $backupPath = [IO.Path]::Combine($BackupDirectory, ($Database + '-recovery-' + [DateTime]::UtcNow.ToString('yyyyMMddTHHmmssZ') + '-' + [guid]::NewGuid().ToString('N') + '.bak'))
    $backupLiteral = Sql-Literal $backupPath
    # Capture every logical file for WITH MOVE; unsupported storage fails before backup/restore.
    $files = @(Invoke-Sql $SourceServer $sourceTrust "SELECT name, type FROM sys.master_files WHERE database_id=DB_ID($databaseLiteral) ORDER BY file_id FOR JSON PATH" -Json)
    if (!$files.Count -or @($files | Where-Object { $_.type -notin @(0,1) }).Count) { throw 'This drill supports ordinary SQL data/log files only; configure a separate FILESTREAM recovery procedure.' }
    Invoke-Sql $SourceServer $sourceTrust "DECLARE @exists int; EXEC master.dbo.xp_fileexist $backupLiteral, @exists OUTPUT; IF @exists=1 THROW 51000, 'Backup file already exists.', 1; BACKUP DATABASE $sourceId TO DISK=$backupLiteral WITH COPY_ONLY, CHECKSUM, NOINIT; RESTORE VERIFYONLY FROM DISK=$backupLiteral WITH CHECKSUM;"
    Write-Output "Retained checksum-protected backup: $backupPath"

    $recoveryLiteral = Sql-Literal $recoveryName
    Invoke-Sql $RecoveryServer $recoveryTrust "IF DB_ID($recoveryLiteral) IS NOT NULL THROW 51000, 'Recovery database already exists; refusing to overwrite.', 1;"
    if (!$RestoreDirectory) { $RestoreDirectory = (Invoke-Sql $RecoveryServer $recoveryTrust "SELECT CAST(SERVERPROPERTY('InstanceDefaultDataPath') AS nvarchar(4000)) AS Path FOR JSON PATH" -Json).Path }
    if (![IO.Path]::IsPathFullyQualified($RestoreDirectory)) { throw 'Specify an absolute data directory on the recovery SQL server.' }
    $moves = @(); $index = 0
    foreach ($file in $files) {
        $extension = if ($file.type -eq 1) { '.ldf' } else { '.mdf' }
        $destination = [IO.Path]::Combine($RestoreDirectory, ($recoveryName + '-' + (++$index) + $extension))
        $moves += 'MOVE ' + (Sql-Literal $file.name) + ' TO ' + (Sql-Literal $destination)
    }
    $restoreAttempted = $true
    Invoke-Sql $RecoveryServer $recoveryTrust "RESTORE DATABASE $recoveryId FROM DISK=$backupLiteral WITH CHECKSUM, RECOVERY, $($moves -join ', '); DBCC CHECKDB ($recoveryId) WITH NO_INFOMSGS, ALL_ERRORMSGS;"
    # Record restored module totals, without exposing patient names or document contents.
    $counts = Invoke-Sql $RecoveryServer $recoveryTrust "SELECT s.name AS [Schema], t.name AS [Table], SUM(p.rows) AS [Rows] FROM $recoveryId.sys.tables t JOIN $recoveryId.sys.schemas s ON t.schema_id=s.schema_id JOIN $recoveryId.sys.partitions p ON t.object_id=p.object_id AND p.index_id IN (0,1) WHERE t.is_ms_shipped=0 GROUP BY s.name,t.name ORDER BY s.name,t.name FOR JSON PATH" -Json
    Write-Output 'PASS: backup checksum verification, isolated restore and full DBCC CHECKDB.'
    $counts | Format-Table -AutoSize | Out-String | Write-Output
} finally {
    # The target name is generated inside this invocation and cannot be supplied by a caller.
    if ($restoreAttempted) {
        Invoke-Sql $RecoveryServer $recoveryTrust "IF DB_ID($recoveryLiteral) IS NOT NULL BEGIN ALTER DATABASE $recoveryId SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE $recoveryId; END"
        Write-Output "Removed disposable recovery database: $recoveryName"
    }
}
