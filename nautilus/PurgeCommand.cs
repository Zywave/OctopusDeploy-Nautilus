using CommandLine;
using Octopus.Client.Model;

namespace Nautilus
{    
    [Verb("purge", HelpText = "Unregisters offline machines in a specified role.")]
    public class PurgeCommand : CommandBase
    {
        [Option('r', "role", Required = true, HelpText = "The machine role for which to purge offline nodes.")]
        public string Role { get; set; }
        
        protected override int Run(OctopusProxy octopus)
        {   
            var machines = octopus.GetMachines(Role);
            foreach (var machine in machines)
            {
                if (machine.Status == MachineModelStatus.Offline)
                {
                    octopus.DeleteMachine(machine);
                    
                    WriteLine($"{machine.Name} is offline and was unregistered successfully");
                }
            }            
            
            return 0;
        }
    }
}