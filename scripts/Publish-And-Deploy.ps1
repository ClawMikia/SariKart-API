$ErrorActionPreference = "Stop"

$RepoDir             = "C:\Users\chris\OneDrive\Desktop\SariKart-API"
$ProjectFile         = Join-Path $RepoDir "SariKartAPI.NET6.csproj"
$PublishDir          = Join-Path $RepoDir "publish"
$MsDeploy            = "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe"
$PublishSettingsPath = Join-Path $RepoDir "infra\sarikart.runasp.net-WebDeploy.publishSettings"

Set-Location -LiteralPath $RepoDir

# Clean previous build/publish output
Remove-Item -Recurse -Force (Join-Path $RepoDir "bin") -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force (Join-Path $RepoDir "obj") -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force $PublishDir                   -ErrorAction SilentlyContinue

dotnet build $ProjectFile -c Release

dotnet publish $ProjectFile -c Release -r win-x64 --self-contained true -o $PublishDir

# Read deploy settings from the .publishSettings file (avoids hardcoding credentials)
[xml]$pub = Get-Content -LiteralPath $PublishSettingsPath
$profile    = $pub.publishData.publishProfile
$publishUrl = $profile.publishUrl
$siteName   = $profile.msdeploySite
$userName   = $profile.userName
$userPWD    = $profile.userPWD

# Ensure out-of-process hosting (matches CI) + stdout logging
$cfg = Join-Path $PublishDir "web.config"
if (Test-Path $cfg) {
    (Get-Content $cfg) `
        -replace 'stdoutLogEnabled="false"', 'stdoutLogEnabled="true"' `
        -replace 'hostingModel="inprocess"', 'hostingModel="outofprocess"' `
        | Set-Content $cfg
}
New-Item -ItemType Directory -Force -Path (Join-Path $PublishDir "logs") | Out-Null

# Pre-check: fail fast if the Web Deploy host is unreachable; try 8172 then 443
$deployPort = $null
foreach ($p in @(8172, 443)) {
    Write-Host "Checking connectivity to $publishUrl`:$p ..."
    if (Test-NetConnection -ComputerName $publishUrl -Port $p -InformationLevel Quiet) {
        $deployPort = $p
        break
    }
}
if (-not $deployPort) {
    Write-Error "Cannot reach $publishUrl on ports 8172 or 443. Check the host, firewall, or network before deploying."
    exit 1
}
Write-Host "Connectivity OK on port $deployPort."

$portSuffix = if ($deployPort -eq 443) { "" } else { ":$deployPort" }
$computerName = "https://$publishUrl$portSuffix/msdeploy.axd?site=$siteName"

& $MsDeploy `
    -verb:sync `
    -source:contentPath=$PublishDir `
    -dest:iisApp=$siteName,computerName=$computerName,userName=$userName,password=$userPWD,authType='Basic' `
    -allowUntrusted `
    -enableRule:AppOffline `
    -useCheckSum

# Clean up local publish artifacts after a successful deploy
Remove-Item -Recurse -Force $PublishDir -ErrorAction SilentlyContinue
