$release = Invoke-RestMethod -Uri "https://api.github.com/repos/Zywave/OctopusDeploy-Nautilus/releases/latest"
$asset = $release.assets | Where-Object { $_.name -eq "nautilus.exe" }
(New-Object System.Net.WebClient).DownloadFile($asset.browser_download_url, ".\nautilus.exe")
