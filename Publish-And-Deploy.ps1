cd C:\Users\chris\OneDrive\Desktop\SariKart-API

dotnet build

Remove-Item -Recurse -Force "bin\Release"
Remove-Item -Recurse -Force "bin\debug"

Remove-Item -Recurse -Force "publish"

Remove-Item -Recurse -Force ".vscode"

dotnet publish -c Release -o ./publish

& "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe" `
-verb:sync `
-source:contentPath='C:\Users\chris\OneDrive\Desktop\SariKart-API\publish' `
-dest:contentPath='site79298',computerName='https://site79298.siteasp.net:8172/msdeploy.axd?site=site79298',userName='site79298',password='Ed2?-8Wie!5Y',authType='Basic' `
-allowUntrusted `
-enableRule:AppOffline `
-useCheckSum

Remove-Item -Recurse -Force "bin\Release"
Remove-Item -Recurse -Force "bin\debug"
 
Remove-Item -Recurse -Force "publish"

Remove-Item -Recurse -Force ".vscode"
