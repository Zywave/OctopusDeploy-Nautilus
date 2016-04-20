using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using CommandLine;
using Microsoft.Win32;

namespace Nautilus
{    
    [Verb("install", HelpText = "Installs an Octopus Tentacle on the local machine.")]
    public class InstallCommand : CommandBase
    {
        [Option('l', "location", Required = false, HelpText = "The install directory of the Octopus Tentacle. Defaults to \"%PROGRAMFILES%\\Octopus Deploy\\Tentacle\".")]
        public string InstallLocation { get; set; }
        
        [Option('h', "home", Required = false, HelpText = "The home directory of the Octopus Tentacle. Defaults to \"%SYSTEMDRIVE%\\Octopus\".")]
        public string HomeLocation { get; set; }
        
        [Option('a', "app", Required = false, HelpText = "The application directory of the Octopus Tentacle. Defaults to \"<home>\\Applications\".")]
        public string AppLocation { get; set; }
        
        [Option('t', "thumbprint", Required = false, HelpText = "The Octopus Server thumbprint. Defaults to global certificate thumbprint.")]
        public string Thumbprint { get; set; }      
               
        [Option('p', "port", Required = false, HelpText = "The port of the Octopus Tentacle. Defaults to 10933.")]
        public int? Port { get; set; }
        
        protected override int Run(OctopusProxy octopus)
        {            
            var installLocation = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Octopus\Tentacle", "InstallLocation", null) as string;
            if (installLocation != null)
            {
                WriteLine("Octopus Tentacle is already installed.");
                return 0;
            }
            
            installLocation = InstallLocation ?? Path.Combine(Environment.GetEnvironmentVariable("PROGRAMFILES"), @"Octopus Deploy\Tentacle");
            
            var homeLocation = HomeLocation ?? Environment.GetEnvironmentVariable("SYSTEMDRIVE") + @"\Octopus";
            homeLocation = homeLocation.TrimEnd(Path.DirectorySeparatorChar);
            
            var appLocation = AppLocation ?? Path.Combine(homeLocation, "Applications");
            appLocation = appLocation.TrimEnd(Path.DirectorySeparatorChar);
            
            var thumbprint = Thumbprint;
            if (thumbprint == null)
            {
                var certicate = octopus.GetGlobalCertificate();
                thumbprint = certicate.Thumbprint;
            }
                        
            var port = Port ?? 10933;
            
            var systemInfo = octopus.GetSystemInfo();        
            var downloadVersion = systemInfo.Version;            
            if (Environment.Is64BitOperatingSystem)
            {
                downloadVersion += "-x64";
            }            
            var downloadUrl = $"http://download.octopusdeploy.com/octopus/Octopus.Tentacle.{downloadVersion}.msi";            
            var filePath = $"{Path.GetTempPath()}Octopus.Tentacle.{downloadVersion}.msi";
            
            Write($"Downloading installer from {downloadUrl} to {filePath}... ");
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(downloadUrl, filePath);
            }
            WriteLine("done");       
             
            Write($"Installing tentacle from {filePath}... ");
            var output = new StringBuilder();
            if (RunProcess("msiexec", $"INSTALLLOCATION=\"{installLocation}\" /i \"{filePath}\" /quiet", output))
            {                      
                WriteLine("done");                
                WriteLine(output.ToString(), ConsoleColor.DarkGray, 1);
                
                Write("Configuring tentacle... ");            
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
                    WriteLine("done");
                    WriteLine(output.ToString(), ConsoleColor.DarkGray, 1);
                    return 0;
                }
            }
            
            WriteLine("failed", ConsoleColor.Red);
            WriteLine(output.ToString(), ConsoleColor.DarkGray, 1);            
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