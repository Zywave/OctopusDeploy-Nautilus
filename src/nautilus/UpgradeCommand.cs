using System;
using CommandLine;
using Octopus.Client.Model.Endpoints;

namespace Nautilus
{
    [Verb("upgrade", HelpText = "Upgrades the Octopus Tentacle on the target machine.")]
    public class UpgradeCommand : CommandBase
    {
        [Option('n', "name", Required = false, HelpText = "The target machine name. Defaults to the local machine name.")]
        public string MachineName { get; set; }
        
        protected override int Run(OctopusProxy octopus)
        {                
            var machineName = MachineName ?? Environment.MachineName;
                        
            var machine = octopus.GetMachine(machineName);
            if (machine == null)
            {
                WriteLine($"Error: The target machine ({machineName}) is not registered with Octopus ({OctopusServerAddress})");
                return 1;
            }
            
            var tentacleVersionDetails = (machine.Endpoint as ListeningTentacleEndpointResource)?.TentacleVersionDetails;            
            if (tentacleVersionDetails != null && !tentacleVersionDetails.UpgradeLocked && (tentacleVersionDetails.UpgradeSuggested || tentacleVersionDetails.UpgradeRequired))
            {
                Write("Updating Tentacle on target machine... ");
                var task = octopus.ExecuteTentacleUpgrade(machine.Id);
                if (task.FinishedSuccessfully) 
                {
                    WriteLine("succeeded", ConsoleColor.Green);
                }
                else
                {
                    WriteLine("failed", ConsoleColor.Red);
                    WriteLine(task.ErrorMessage);
                }
            }
            else 
            {
                WriteLine("Tentacle is up to date");
            }
            
            if (!machine.HasLatestCalamari)
            {
                Write("Updating Calamari on target machine... ");
                var task = octopus.ExecuteCalamariUpdate(machine.Id);
                if (task.FinishedSuccessfully) 
                {
                    WriteLine("succeeded", ConsoleColor.Green);
                }
                else
                {
                    WriteLine("failed", ConsoleColor.Red);
                    WriteLine(task.ErrorMessage);
                }
            }
            else 
            {
                WriteLine("Calamari is up to date");
            }
            
            return 0;
        }
    }
}