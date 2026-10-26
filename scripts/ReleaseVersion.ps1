# Shared release policy for PR preparation, CI and manual publication. No network or mutations.
Set-StrictMode -Version Latest

function ConvertFrom-ReleaseVersionXml([string]$Content) {
    $settings = [Xml.XmlReaderSettings]::new()
    $settings.DtdProcessing = [Xml.DtdProcessing]::Prohibit
    $reader = [Xml.XmlReader]::Create([IO.StringReader]::new($Content), $settings)
    try { $document = [Xml.XmlDocument]::new(); $document.Load($reader) } finally { $reader.Dispose() }
    $versions = $document.SelectNodes('/Project/PropertyGroup/EgcareReleaseVersion')
    $builds = $document.SelectNodes('/Project/PropertyGroup/EgcareReleaseBuild')
    if ($versions.Count -ne 1 -or $builds.Count -ne 1) { throw 'ReleaseVersion.props must contain exactly one version and build number.' }
    $version = $versions[0].InnerText
    $build = $builds[0].InnerText
    if ($version -notmatch '^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$') { throw 'Release version must be numeric major.minor.patch without leading zeros.' }
    if (@($version.Split('.') | Where-Object { [long]$_ -gt 65534 }).Count) { throw 'Release version components must be between 0 and 65534.' }
    if ($build -notmatch '^[1-9][0-9]*$' -or [long]$build -gt 65535) { throw 'Release build must be between 1 and 65535.' }
    [pscustomobject]@{ Version = $version; Build = [int]$build }
}

function Get-GitReleaseVersion([string]$Ref, [switch]$AllowMissing) {
    $sha = git rev-parse --verify "$Ref^{commit}"
    if ($LASTEXITCODE -ne 0) { throw "Cannot resolve release ref: $Ref" }
    $path = git ls-tree --name-only $sha -- ReleaseVersion.props
    if ($LASTEXITCODE -ne 0) { throw "Cannot inspect release ref: $Ref" }
    if (!$path) {
        if ($AllowMissing) { return $null }
        throw "ReleaseVersion.props is missing at $Ref."
    }
    $content = git show "${sha}:ReleaseVersion.props"
    if ($LASTEXITCODE -ne 0) { throw "Cannot read release version at $Ref." }
    ConvertFrom-ReleaseVersionXml ($content -join "`n")
}

function Assert-ReleaseTransition($Candidate, $Production, [switch]$AllowUnpublishedRepair) {
    if ($null -eq $Production) {
        if ($Candidate.Version -ne '1.0.0' -or $Candidate.Build -ne 1) { throw 'The first tracked release must be 1.0.0, build 1.' }
        return
    }
    # Callers may opt in only after confirming this version has no GitHub release or draft.
    if ($AllowUnpublishedRepair -and $Candidate.Version -ceq $Production.Version -and $Candidate.Build -eq $Production.Build) { return }
    if ([version]$Candidate.Version -le [version]$Production.Version) { throw "Development version $($Candidate.Version) must be newer than main version $($Production.Version)." }
    if ($Candidate.Build -le $Production.Build) { throw "Development build $($Candidate.Build) must be newer than main build $($Production.Build)." }
}

function Assert-ReleaseInputs($Release, [string]$Version, [string]$Build) {
    if ($Version -cne $Release.Version) { throw "Pipeline version must match the merged ReleaseVersion.props: $($Release.Version)." }
    if ($Build -ne '0' -and $Build -cne [string]$Release.Build) { throw "Pipeline build must be 0 (use tracked build) or $($Release.Build)." }
}
