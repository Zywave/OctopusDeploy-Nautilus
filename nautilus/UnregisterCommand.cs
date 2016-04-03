using System;
using CommandLine;

namespace Nautilus
{    
    [Verb("unregister", HelpText = "Unregisters the target machine from Octopus.")]
    public class UnregisterCommand : CommandBase
    {
        [Option('n', "name", Required = false, HelpText = "The target machine name.")]
        public string MachineName { get; set; }
        
        protected override int Run(OctopusProxy octopus)
        {            
            var machineName = MachineName ?? Environment.MachineName;
            
            var machine = octopus.GetMachine(machineName);
            if (machine == null)
            {
                Console.WriteLine($"The target machine ({machineName}) is not registered with Octopus ({OctopusServerAddress})");
                return 0;
            }
            
            octopus.DeleteMachine(machine);
            
            Console.WriteLine($"Target machine ({machine.Name}) unregistered successfully");
            
            return 0;
        }
    }
}