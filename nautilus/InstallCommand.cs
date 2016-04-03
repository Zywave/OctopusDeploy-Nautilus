using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using CommandLine;

namespace Nautilus
{    
    [Verb("install", HelpText = "Installs an Octopus tenticle on the local machine.")]
    public class InstallCommand : CommandBase
    {
        protected override int Run(OctopusProxy octopus)
        {   
            var systemInfo = octopus.GetSystemInfo();            
            var downloadVersion = systemInfo.Version;            
            if (Environment.Is64BitOperatingSystem)
            {
                downloadVersion += "-x64";
            }            
            var downloadUrl = $"http://download.octopusdeploy.com/octopus/Octopus.Tentacle.{downloadVersion}.msi";            
            var filePath = $"{Path.GetTempPath()}Octopus.Tentacle.{downloadVersion}.msi";
            
            Console.WriteLine($"Downloading installer from {downloadUrl} to {filePath}");
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(downloadUrl, filePath);
            }
             
            Console.WriteLine($"Installing tentacle from {filePath}");
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "msiexec",
                Arguments = $"/i \"{filePath}\" /quiet",
                UseShellExecute = true
            };
            using (var process = Process.Start(processStartInfo))
            {
                var timeout = 120000;
                if (!process.WaitForExit(timeout))
                {
                    Console.WriteLine($"Error: Operation timed out ({timeout} milliseconds) while waiting for tentacle installation to complete");
                    return 1;
                }
                
                if (process.ExitCode != 0)
                {                    
                    Console.WriteLine($"Tentacle installation failed and exited with code {process.ExitCode}");
                    return 1;
                }
            }
            
            Console.WriteLine("Tentacle installation completed successfully");
            
            Console.WriteLine("Configuring tentacle");
            
            //todo: configure
            
//             cd "C:\Program Files\Octopus Deploy\Tentacle"
 
 
// Tentacle.exe create-instance --instance "Tentacle" --config "C:\Octopus\Tentacle.config" --console
// Tentacle.exe new-certificate --instance "Tentacle" --if-blank --console
// Tentacle.exe configure --instance "Tentacle" --reset-trust --console
// Tentacle.exe configure --instance "Tentacle" --home "C:\Octopus" --app "C:\Octopus\Applications" --port "10933" --console
// Tentacle.exe configure --instance "Tentacle" --trust "YOUR_OCTOPUS_THUMBPRINT" --console
// "netsh" advfirewall firewall add rule "name=Octopus Deploy Tentacle" dir=in action=allow protocol=TCP localport=10933
// Tentacle.exe register-with --instance "Tentacle" --server "http://YOUR_OCTOPUS" --apiKey="API-YOUR_API_KEY" --role "web-server" --environment "Staging" --comms-style TentaclePassive --console
// Tentacle.exe service --instance "Tentacle" --install --start --console
            
            return 0;
        }
    }
}