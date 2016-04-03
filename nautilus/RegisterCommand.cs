using System;
using System.Collections.Generic;
using CommandLine;

namespace Nautilus
{    
    [Verb("register", HelpText = "Registers the target machine with Octopus.")]
    public class RegisterCommand : CommandBase
    {
        [Option('n', "name", Required = false, HelpText = "The target machine name.")]
        public string MachineName { get; set; }
        
        [Option('t', "thumbprint", Required = false, HelpText = "The Octopus server certificate thumbprint. Defaults to global certificate.")]
        public string Thumbprint { get; set; }        
        
        [Option('h', "host", Required = false, HelpText = "The target machine tentacle host name.")]
        public string HostName { get; set; }
        
        [Option('p', "port", Required = false, HelpText = "The target machine tentacle port.")]
        public int? Port { get; set; }
        
        [Option('e', "environments", Required = true, HelpText = "The environment names of the target machine.")]
        public IList<string> Environments { get; set; }
        
        [Option('r', "roles", Required = true, HelpText = "The roles of the target machine.")]
        public IList<string> Roles { get; set; }
        
        protected override int Run(OctopusProxy octopus)
        {                                    
            var machineName = MachineName ?? Environment.MachineName;
            
            var machine = octopus.GetMachine(machineName);
            if (machine != null)
            {
                Console.WriteLine($"The target machine ({machineName}) is already registered with Octopus ({OctopusServerAddress})");
                return 0;
            }
            
            var hostName = HostName ?? Environment.MachineName;
            var port = Port ?? 10933;
            
            var thumbprint = Thumbprint;
            if (thumbprint == null)
            {
                var certicate = octopus.GetGlobalCertificate();
                thumbprint = certicate.Thumbprint;
            }
            
            machine = octopus.CreateMachine(machineName, thumbprint, hostName, port, Environments, Roles);
            
            Console.WriteLine($"Target machine ({machine.Name}) registered successfully");
            
            return 0;
        }
    }
}