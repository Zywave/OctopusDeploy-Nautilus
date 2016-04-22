using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace Nautilus
{    
    public partial class NautilusService
    {   
        public void Register(IEnumerable<string> environments, IEnumerable<string> roles, string machineName = null, string thumbprint = null, string hostName = null, int? port = null, bool update = false)
        {
            if (environments == null) throw new ArgumentNullException(nameof(environments));
            if (!environments.Any()) throw new ArgumentException("At lease one environment must be specified.", nameof(environments));
            if (roles == null) throw new ArgumentNullException(nameof(roles));
            if (!roles.Any()) throw new ArgumentException("At lease one environment must be specified.", nameof(roles));
             
            machineName = machineName ?? Environment.MachineName;            
            hostName = hostName ?? Environment.MachineName;
            port = port ?? 10933;
                        
            if (thumbprint == null)
            {
                thumbprint = GetTentacleThumbprint();
                if (thumbprint == null)
                {
                    var message = "Could not determine thumbprint because an Octopus Tentacle is not installed on this machine";
                    Log.WriteLine($"Error: {message}");
                    throw new NautilusException(NautilusErrorCodes.TentacleNotInstalled, message);
                }
            }
            
            var machine = Octopus.GetMachine(machineName);
            if (machine != null)
            {
                Log.WriteLine($"The machine ({machineName}) is already registered with Octopus");
                
                if (update)
                {
                    machine = Octopus.ModifyMachine(machine, thumbprint, hostName, port.Value, environments, roles);
                    
                    Log.WriteLine($"Registration updated successfully");
                }
                
                return;
            }
            
            machine = Octopus.CreateMachine(machineName, thumbprint, hostName, port.Value, environments, roles);
            
            Log.WriteLine($"The machine ({machine.Name}) was registered successfully");
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