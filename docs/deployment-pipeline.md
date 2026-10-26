# EGcare Windows releases and updates

EGcare remains one Windows application. SQL and PACS stay on the clinic LAN. Application updates may use the internet: the default feed is the public source repository's latest stable release at `https://github.com/AhmedAFarhan/radiology-center-management/releases/latest/download`. No GitHub token, SQL credential or clinic data is embedded in the updater.

## Prepare a release

1. Work on `development`. Before preparing a release, merge `main` back into `development`, then update the root `ReleaseVersion.props` and add `docs/releases/VERSION.md`. Both the semantic version and build number must exceed those on `main`. The initial tracked release is **1.0.0, build 1**; the older unversioned main branch is treated as the bootstrap baseline, not an already-published 1.0.0 release.
2. Commit and push the reviewed development changes, then run `./scripts/New-ReleasePullRequest.ps1`. It compares the two remote branches and published releases, creates or refreshes one `development` → `main` PR, and includes the exact manual pipeline parameters. It uses an existing Git Credential Manager sign-in or the process's `GH_TOKEN`/`GITHUB_TOKEN`; it never stores credentials, merges, creates a tag or starts deployment. CI checks that the release comes from this repository's `development` branch and increases both tracked numbers. Review and merge using **Create a merge commit** or **Squash and merge**. Keep both permanent branches; do not delete `development`.
3. Open GitHub **Actions → Package and release EGcare → Run workflow**. Choose `main`, enter the version from the merged PR, and leave **publish_release** unchecked for the first review build. Enter the PR's build number, or use `0` to read it from `ReleaseVersion.props`. The workflow rejects different version/build inputs and requires main to be the release PR's merge commit.
4. The workflow validates inputs, runs the complete application CI, publishes the self-contained Windows app, rejects private/configuration/database files, downloads the previous release when present, and packs the installer/full package/delta update with pinned Velopack 1.2.0.
5. Download the `EGcare-win-VERSION` workflow artifact and verify installation, the visible app version, clinic sign-in and updating from the previous installed version on staging PCs. Review artifacts may be unsigned when no signing credentials are configured. They are not production releases.
6. After acceptance, manually run the same workflow/version with **publish_release** checked. A push, tag or PR merge never deploys automatically. Publication creates the release tag; do not create it in advance. Merge `main` back into `development` before the next release, then increase both numbers and prepare a new PR.

For the first release after its PR is merged:

| Manual input | Review artifact | Production publication |
|---|---|---|
| Branch | `main` | `main` |
| `version` | `1.0.0` | `1.0.0` |
| `build_number` | `1` | `1` |
| `publish_release` | `false` | `true` |

Later releases follow the same sequence. Updating the version file alone does not open a PR: run the preparation script (or ask Codex to prepare the next release) after committing/pushing. An existing open release PR is refreshed instead of duplicated. Enable the **Compare release versions** and application test/build checks in main's branch protection if you want GitHub to enforce successful checks before merging; this guide does not change repository protection settings automatically.

Publishing uses this repository's `GITHUB_TOKEN` with Contents write permission only in the publishing job. No separate release repository, GitHub App or personal access token is required. Uploads first go into a draft; only after all assets are uploaded is the release made public/latest, so clients never discover a partially uploaded feed. A failed publish can leave a draft for manual inspection; the workflow refuses to overwrite it automatically.

The tracked `ReleaseVersion.props` also supplies the version for ordinary Visual Studio builds. The application display version, assembly/file version, Velopack package version and GitHub tag use the same reviewed release version. The app displays it on the login form, clinic setup header, expanded sidebar, profile menu and update dialog. The installed/current and available versions use left-to-right number rendering in both Arabic and English.

## Code signing

Code signing is optional. With no signing secrets configured, both review artifacts and production releases are unsigned. No certificate is required to publish or update EGcare. If you choose to sign later, configure **one** method in this repository's Actions secrets:

| Method | Secrets | Requirement |
|---|---|---|
| Exportable code-signing certificate | `EGCARE_SIGN_PFX_BASE64`, `EGCARE_SIGN_PFX_PASSWORD` | A trusted code-signing certificate/private key in PFX format. The workflow imports it into the ephemeral runner's CurrentUser store, timestamps signatures and removes the imported certificate/PFX afterward. |
| Existing signing provider | `EGCARE_SIGN_TEMPLATE` | Velopack custom signing command containing `{{file}}`. Configure the provider's required tools/authentication on the runner before the pack step. |

Never commit a PFX/password or embed signing credentials in the app. A certificate stored on a hardware device or cloud signing provider may not support PFX export; use that provider's signing integration. When signing is configured, the workflow validates both the installer and packaged app signatures and requires their certificates to match; signing failures stop the release rather than silently falling back to unsigned files. With signing absent, publication verifies release-file SHA-256 checksums.

Unsigned installers may show an unknown-publisher/SmartScreen prompt, and managed Windows policies may block them. Include this in the intended-PC installation check; see [Microsoft's SmartScreen guidance](https://learn.microsoft.com/windows/apps/package-and-deploy/smartscreen-reputation).

## Installed application behavior

- On launch and every five minutes, the app checks for a newer release. Automatic network failures stay quiet and do not block clinic work. A manual check shows a readable Arabic/English error.
- A new release produces a nonmodal notification with **View update**. It does not interrupt a patient form or force a restart. Manual update entry points remain available on login, the profile menu and the failed-startup screen, including when SQL cannot be reached or requires maintenance.
- **Download update** shows progress and supports cancellation. A completed download changes the action to **Install and restart**. Closing the dialog postpones installation. The app never automatically applies a downloaded update at startup.
- Before restart, EGcare drains in-flight business operations and releases its shared database maintenance lease. Clinic configuration and the DPAPI-protected saved session remain outside the installation directory. Existing expiry/revalidation rules still apply after restart.
- Feed changes invalidate an earlier offer/download; the user must check the selected source again. Checks/downloads/restarts are serialized. A cancelled or failed download cannot be installed.
- Visual Studio builds and unpackaged portable copies do not self-update. Use the Velopack-installed copy to test installation/update behavior.

## Offline clinic share

An explicitly configured `UpdatesDirectory` in the public clinic connection file overrides GitHub. There is no internet fallback when that share is unavailable; this preserves the offline-PC choice. Clear the optional directory to return to GitHub updates. SQL/PACS addresses do not change.

Download all release assets into a directory on an administrator PC. Keep `Publish-ClinicUpdateFeed.ps1` and `Assert-ReleaseSignatures.ps1` together, then run:

```powershell
./Publish-ClinicUpdateFeed.ps1 -ReleaseDirectory C:\EGcareReleases\1.1.0 -Destination '\\ADMIN-PC\EGcareUpdates'
```

The script verifies release identity and package sizes/SHA-256. Unsigned clinic updates are supported by default; use `-RequireSignature` only if the clinic later adopts signed releases. It serializes promotion with a share lock, copies immutable packages first, atomically replaces the setup file, and switches `releases.win.json` last. It rejects downgrades, path traversal, missing/corrupt files and changed contents for an existing version. Give staff read access and only release administrators write access. Old packages are retained so a client already downloading an earlier feed is not interrupted.

Hashes detect corrupted or changed files; they do not identify a publisher. Distribute release assets through this repository or the administrator-controlled clinic share.

## Prerequisites and database upgrades

The installer includes .NET and Windows App SDK. WebView2 must be installed on the target PC; include Microsoft's offline WebView2 installer in the clinic setup kit for PCs without internet. The workflow deliberately does not make an offline install depend on downloading a runtime.

An application update does not run database migrations. For a database-changing release: save work and choose **Exit** from the tray on every PC, update the admin app, perform backed-up **Admin PC setup / upgrade**, then update/reconnect the workstations. Closing a window to the tray does not disconnect its database lease. Incompatible clients are stopped by the existing schema/security checks and can still reach the updater.

## Local verification

```powershell
dotnet tool restore
dotnet test src/Desktop/RadiologyCenter.Desktop.StartupTests/RadiologyCenter.Desktop.StartupTests.csproj -c Release
dotnet test src/Desktop/RadiologyCenter.Desktop.ComponentTests/RadiologyCenter.Desktop.ComponentTests.csproj -c Release
./scripts/Test-ReleaseTooling.ps1
dotnet publish src/Desktop/RadiologyCenter.Desktop/RadiologyCenter.Desktop.csproj -c Release -f net10.0-windows10.0.19041.0 -r win-x64 --self-contained true -p:WindowsPackageType=None -p:WindowsAppSDKSelfContained=true -p:ApplicationDisplayVersion=1.1.0 -p:ApplicationVersion=2 -p:AssemblyVersion=1.1.0.0 -p:FileVersion=1.1.0.0 -o audit-results/release-app
./scripts/Assert-ReleasePayload.ps1 -AppDirectory audit-results/release-app
dotnet tool run vpk pack -u RadiologyCenter --packTitle EGcare -v 1.1.0 -p audit-results/release-app --mainExe RadiologyCenter.Desktop.exe --channel win --icon src/Desktop/RadiologyCenter.Desktop/Resources/taskbar.ico -o audit-results/local-release
```

The local packaging example creates unsigned staging artifacts only. Keep package ID `RadiologyCenter`, channel `win`, and executable `RadiologyCenter.Desktop.exe` unchanged so existing installed copies recognize upgrades. Official references: [Velopack integration](https://docs.velopack.io/integrating/overview), [update sources](https://docs.velopack.io/integrating/update-sources), [runtime prerequisites](https://docs.velopack.io/packaging/bootstrapping).
