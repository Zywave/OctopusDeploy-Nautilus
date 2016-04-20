using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Win32;

namespace Nautilus
{    
    public partial class NautilusService
    {   
        public int Install(string installLocation = null, string homeLocation = null, string appLocation = null, string thumbprint = null, int? port = null)
        {            
            var existingInstallLocation = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Octopus\Tentacle", "InstallLocation", null) as string;
            if (existingInstallLocation != null)
            {
                Log.WriteLine("Octopus Tentacle is already installed.");
                return 0;
            }
            
            installLocation = installLocation ?? Path.Combine(Environment.GetEnvironmentVariable("PROGRAMFILES"), @"Octopus Deploy\Tentacle");
            
            homeLocation = homeLocation ?? Environment.GetEnvironmentVariable("SYSTEMDRIVE") + @"\Octopus";
            homeLocation = homeLocation.TrimEnd(Path.DirectorySeparatorChar);
            
            appLocation = appLocation ?? Path.Combine(homeLocation, "Applications");
            appLocation = appLocation.TrimEnd(Path.DirectorySeparatorChar);
            
            if (thumbprint == null)
            {
                var certicate = Octopus.GetGlobalCertificate();
                thumbprint = certicate.Thumbprint;
            }
                        
            port = port ?? 10933;
            
            var systemInfo = Octopus.GetSystemInfo();        
            var downloadVersion = systemInfo.Version;            
            if (Environment.Is64BitOperatingSystem)
            {
                downloadVersion += "-x64";
            }            
            var downloadUrl = $"http://download.octopusdeploy.com/octopus/Octopus.Tentacle.{downloadVersion}.msi";            
            var filePath = $"{Path.GetTempPath()}Octopus.Tentacle.{downloadVersion}.msi";
            
            Log.Write($"Downloading installer from {downloadUrl} to {filePath}... ");
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(downloadUrl, filePath);
            }
            Log.WriteLine("done");       
             
            Log.Write($"Installing tentacle from {filePath}... ");
            var output = new StringBuilder();
            if (RunProcess("msiexec", $"INSTALLLOCATION=\"{installLocation}\" /i \"{filePath}\" /quiet", output))
            {                      
                Log.WriteLine("done");                
                Log.WriteLine(Indent(output.ToString()));
                
                Log.Write("Configuring tentacle... ");            
                var tentacleExe = installLocation + @"\Tentacle.exe";     
                
                var configFilePath = $"{homeLocation}\\Tentacle.config";
                if (File.Exists(configFilePath))
                {
                    File.Delete(configFilePath);
                }
                
                output.Clear(); 
                if (RunProcess(tentacleExe, $"create-instance --instance \"Tentacle\" --config \"{configFilePath}\" --console", output))
                if (RunProcess(tentacleExe, $"new-certificate --instance \"Tentacle\" --if-blank --console", output))
                if (RunProcess(tentacleExe, $"configure --instance \"Tentacle\" --reset-trust --console", output))
                if (RunProcess(tentacleExe, $"configure --instance \"Tentacle\" --home \"{homeLocation}\" --app \"{appLocation}\" --port \"{port}\" --console", output))
                if (RunProcess(tentacleExe, $"configure --instance \"Tentacle\" --trust \"{thumbprint}\" --console", output))
                if (RunProcess("netsh", $"advfirewall firewall add rule \"name=Octopus Deploy Tentacle\" dir=in action=allow protocol=TCP localport={port}", output))
                if (RunProcess(tentacleExe, $"service --instance \"Tentacle\" --install --start --console", output))
                {
                    Log.WriteLine("done");
                    Log.WriteLine(Indent(output.ToString()));
                    return 0;
                }
            }
            
            Log.WriteLine("failed");
            Log.WriteLine(Indent(output.ToString()));            
            return 1;
        }
        
        
        private static bool RunProcess(string fileName, string arguments, StringBuilder output)
        {
            output.AppendLine($"{fileName} {arguments}");
            
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            using (var process = Process.Start(startInfo))
            {
                const int timeout = 120000;
                if (!process.WaitForExit(timeout))
                {
                    output.AppendLine($"Error: Operation timed out ({timeout} milliseconds)");
                    return false;
                }
                                    
                if (process.ExitCode != 0)
                {                    
                    output.AppendLine($"Error: Operation failed and exited with code {process.ExitCode}");
                    output.AppendLine(process.StandardError.ReadToEnd());
                    return false;
                }
                
                output.AppendLine(process.StandardOutput.ReadToEnd());                
                return true;
            }
        }
    }
}