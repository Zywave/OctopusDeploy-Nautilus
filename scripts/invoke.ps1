$release = Invoke-RestMethod -Uri "https://api.github.com/repos/Zywave/OctopusDeploy-Nautilus/releases/latest"
$asset = $release.assets | Where-Object { $_.name -eq "nautilus.exe" }
(New-Object System.Net.WebClient).DownloadFile($asset.browser_download_url, ".\nautilus.exe")

.\nautilus.exe install -s $env:OctopusServerAddress -k $env:OctopusAPIKey -h "$env:OctopusHomePath"
If (-Not $?) { Exit $LastExitCode }

.\nautilus.exe register -s $env:OctopusServerAddress -k $env:OctopusAPIKey -e $env:OctopusEnvironment -r $env:OctopusRole -u
If (-Not $?) { Exit $LastExitCode }

.\nautilus.exe upgrade -s $env:OctopusServerAddress -k $env:OctopusAPIKey
If (-Not $?) { Exit $LastExitCode }

.\nautilus.exe deploy -s $env:OctopusServerAddress -k $env:OctopusAPIKey -w -f
If (-Not $?) { Exit $LastExitCode }