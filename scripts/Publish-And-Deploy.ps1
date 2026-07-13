$ErrorActionPreference = "Stop"

$ProjectDir         = "C:\Users\chris\OneDrive\Desktop\SariKart-API\SariKartAPI"
$PublishDir         = Join-Path $ProjectDir "publish"
$MsDeploy           = "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe"
$PublishSettingsPath = "C:\Users\chris\OneDrive\Desktop\SariKart-API\infra\sarikart.runasp.net-WebDeploy.publishSettings"

Set-Location -LiteralPath $ProjectDir

dotnet build -c Release

dotnet publish -c Release -o $PublishDir

# Read deploy settings from the .publishSettings file (avoids hardcoding credentials)
[xml]$pub = Get-Content -LiteralPath $PublishSettingsPath
$profile   = $pub.publishData.publishProfile
$publishUrl = $profile.publishUrl
$siteName   = $profile.msdeploySite
$userName   = $profile.userName
$userPWD    = $profile.userPWD

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

