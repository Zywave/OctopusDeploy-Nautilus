using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CommandLine;
using Microsoft.Win32;

namespace Nautilus
{    
    [Verb("register", HelpText = "Registers the local machine with Octopus.")]
    public class RegisterCommand : CommandBase
    {
        [Option('n', "name", Required = false, HelpText = "The machine name. Defaults to the local machine name.")]
        public string MachineName { get; set; }
        
        [Option('t', "thumbprint", Required = false, HelpText = "The Octopus Tentacle thumbprint. Defaults to the local Tentacle thumbprint.")]
        public string Thumbprint { get; set; }        
        
        [Option('h', "host", Required = false, HelpText = "The Tentacle host name. Defaults to the local machine name.")]
        public string HostName { get; set; }
        
        [Option('p', "port", Required = false, HelpText = "The Tentacle port. Defaults to 10933.")]
        public int? Port { get; set; }
        
        [Option('e', "environments", Required = true, HelpText = "The environment names of the machine.")]
        public IList<string> Environments { get; set; }
        
        [Option('r', "roles", Required = true, HelpText = "The roles of the machine.")]
        public IList<string> Roles { get; set; }
        
        protected override int Run(OctopusProxy octopus)
        {                                    
            var machineName = MachineName ?? Environment.MachineName;            
            var hostName = HostName ?? Environment.MachineName;
            var port = Port ?? 10933;
                        
            var thumbprint = Thumbprint;
            if (thumbprint == null)
            {
                thumbprint = GetTentacleThumbprint();
            }
            
            var machine = octopus.GetMachine(machineName);
            if (machine != null)
            {
                WriteLine($"The machine ({machineName}) is already registered with Octopus ({OctopusServerAddress})");
                return 0;
            }
            
            machine = octopus.CreateMachine(machineName, thumbprint, hostName, port, Environments, Roles);
            
            WriteLine($"The machine ({machine.Name}) was registered successfully");
            
            return 0;
        }
        
        private static string GetTentacleThumbprint()
        {
            var installLocation = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Octopus\Tentacle", "InstallLocation", null) as string;
            if (installLocation == null)
            {
                return null;
            }
            
            var processStartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(installLocation, "Tentacle.exe"),
                Arguments = "show-thumbprint --nologo --console",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            string thumbprint;
            using (var process = Process.Start(processStartInfo))
            {
                thumbprint = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    return null;
                }
                
                thumbprint = thumbprint.Substring(36, 40);
            }
            
            return thumbprint;
        }
    }
}