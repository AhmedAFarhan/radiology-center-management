#requires -Version 7.0
<#
.SYNOPSIS
Creates or refreshes the development-to-main release PR with the approved manual pipeline inputs.
.DESCRIPTION
Commit and push development first. Uses GH_TOKEN/GITHUB_TOKEN or the existing Git credential
manager sign-in. Never merges, tags, dispatches a workflow or publishes a release.
#>
[CmdletBinding()]
param(
    [ValidatePattern('^[A-Za-z0-9_.-]+/[A-Za-z0-9_.-]+$')]
    [string]$Repository = 'AhmedAFarhan/radiology-center-management'
)
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'ReleaseVersion.ps1')
$repoRoot = Split-Path $PSScriptRoot -Parent
Push-Location $repoRoot
try {
    $origin = git remote get-url origin
    if ($LASTEXITCODE -ne 0 -or $origin -notin @("https://github.com/$Repository.git", "https://github.com/$Repository", "git@github.com:$Repository.git")) { throw 'Origin must match the selected GitHub repository.' }
    $branch = git branch --show-current
    if ($branch -ne 'development') { throw 'Prepare release PRs from the development checkout.' }
    $status = git status --porcelain
    if ($status) { throw 'Commit and push the reviewed development changes before preparing the PR.' }
    git fetch origin main development
    if ($LASTEXITCODE -ne 0) { throw 'Cannot refresh main and development.' }
    $localHead = git rev-parse HEAD
    $remoteHead = git rev-parse origin/development
    if ($localHead -ne $remoteHead) { throw 'Local development must match origin/development. Push or synchronize it first.' }
    git merge-base --is-ancestor origin/main origin/development
    if ($LASTEXITCODE -ne 0) { throw 'Merge main back into development before preparing the next release.' }
    $mainHead = git rev-parse origin/main
    if ($mainHead -eq $remoteHead) { throw 'There are no development changes to release.' }
    $production = Get-GitReleaseVersion origin/main -AllowMissing
    $candidate = Get-GitReleaseVersion origin/development
    Assert-ReleaseTransition $candidate $production
    $notesPath = Join-Path $repoRoot "docs/releases/$($candidate.Version).md"
    if (!(Test-Path -LiteralPath $notesPath -PathType Leaf)) { throw "Add reviewed release notes at $notesPath before preparing the PR." }

    $token = $env:GH_TOKEN
    if (!$token) { $token = $env:GITHUB_TOKEN }
    if (!$token) {
        # Keep the existing credential only in memory; never echo it or write it into a file.
        $credentialLines = "protocol=https`nhost=github.com`npath=$Repository.git`n`n" | git -c credential.interactive=false credential fill
        if ($LASTEXITCODE -ne 0) { throw 'Sign into GitHub with Git Credential Manager or supply GH_TOKEN for this process.' }
        foreach ($line in $credentialLines) {
            if ($line.StartsWith('password=')) { $token = $line.Substring(9) }
        }
        $credentialLines = $null
    }
    if (!$token) { throw 'GitHub authentication is required to create the release PR.' }
    $headers = @{ Authorization = "Bearer $token"; 'User-Agent' = 'EGcare-release-preparation'; 'X-GitHub-Api-Version' = '2022-11-28' }
    $api = "https://api.github.com/repos/$Repository"
    $latest = Invoke-WebRequest "$api/releases/latest" -Headers $headers -SkipHttpErrorCheck
    if ($latest.StatusCode -eq 200) {
        $tag = ($latest.Content | ConvertFrom-Json).tag_name
        if ($tag -notmatch '^v(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$' -or [version]$candidate.Version -le [version]$tag.Substring(1)) { throw 'Candidate must be newer than the latest published release.' }
        if (!$production) { throw 'A published release exists but main has no tracked release version. Reconcile main before proceeding.' }
    } elseif ($latest.StatusCode -ne 404) { throw "Release lookup failed: HTTP $($latest.StatusCode)." }
    $existingRelease = Invoke-WebRequest "$api/releases/tags/v$($candidate.Version)" -Headers $headers -SkipHttpErrorCheck
    if ($existingRelease.StatusCode -ne 404) { throw 'This version already has a release/draft, or its lookup failed. Inspect it before proceeding.' }
    $previous = if ($production) { "$($production.Version), build $($production.Build)" } else { 'No tracked/published release (first release)' }
    $notes = Get-Content -LiteralPath $notesPath -Raw
    $body = @"
$notes

## Branch and version comparison

| Branch | Release |
|---|---|
| main | $previous |
| development | $($candidate.Version), build $($candidate.Build) |

## Manual pipeline parameters after merge

Workflow: **Package and release EGcare**. Select branch **main**.

| Parameter | Review artifact | Signed production release |
|---|---|---|
| version | $($candidate.Version) | $($candidate.Version) |
| build_number | $($candidate.Build) | $($candidate.Build) |
| publish_release | false | true |

Merging this PR does not deploy. Run the review artifact manually, complete installation/upgrade acceptance, then run signed publication manually. Signing secrets are required for production publication. Both runs use the same version from ReleaseVersion.props.

After merging, merge main back into development before preparing the next release. Keep development as a permanent branch. Do not delete it after merging.
"@
    $owner = $Repository.Split('/')[0]
    $head = [Uri]::EscapeDataString("${owner}:development")
    $pullResponse = Invoke-RestMethod "$api/pulls?state=open&base=main&head=$head" -Headers $headers
    $pulls = @($pullResponse)
    if ($pulls.Count -gt 1) { throw 'More than one release PR is open; inspect them before continuing.' }
    $title = "Release $($candidate.Version) — development to main"
    if ($pulls.Count -eq 1) {
        $payload = @{ title = $title; body = $body } | ConvertTo-Json
        $pull = Invoke-RestMethod "$api/pulls/$($pulls[0].number)" -Method Patch -Headers $headers -ContentType 'application/json; charset=utf-8' -Body ([Text.Encoding]::UTF8.GetBytes($payload))
    } else {
        $payload = @{ title = $title; body = $body; head = 'development'; base = 'main'; draft = $false } | ConvertTo-Json
        $pull = Invoke-RestMethod "$api/pulls" -Method Post -Headers $headers -ContentType 'application/json; charset=utf-8' -Body ([Text.Encoding]::UTF8.GetBytes($payload))
    }
    [pscustomobject]@{ PullRequest = $pull.html_url; Version = $candidate.Version; Build = $candidate.Build; WorkflowBranch = 'main'; PublishReview = $false; PublishProduction = $true }
} finally {
    $token = $null; $headers = $null; $credentialLines = $null
    Pop-Location
}
