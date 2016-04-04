using System;
using CommandLine;
using Octopus.Client.Model.Endpoints;

namespace Nautilus
{
    [Verb("update", HelpText = "Updates the Octopus Tenticle on the target machine.")]
    public class UpdateCommand : CommandBase
    {
        [Option('n', "name", Required = false, HelpText = "The target machine name. Defaults to the local machine name.")]
        public string MachineName { get; set; }
        
        protected override int Run(OctopusProxy octopus)
        {                
            var machineName = MachineName ?? Environment.MachineName;
                        
            var machine = octopus.GetMachine(machineName);
            if (machine == null)
            {
                Console.WriteLine($"Error: The target machine ({machineName}) is not registered with Octopus ({OctopusServerAddress})");
                return 1;
            }
            
            var tentacleVersionDetails = (machine.Endpoint as ListeningTentacleEndpointResource)?.TentacleVersionDetails;            
            if (tentacleVersionDetails != null && !tentacleVersionDetails.UpgradeLocked && (tentacleVersionDetails.UpgradeSuggested || tentacleVersionDetails.UpgradeRequired))
            {
                Console.WriteLine("Updating Tentacle on target machine");
                var task = octopus.ExecuteTentacleUpgrade(machine.Id);
                if (task.FinishedSuccessfully) 
                {
                    Console.WriteLine("Tentacle update completed successfully");
                }
                else
                {
                    Console.WriteLine(task.ErrorMessage);
                    Console.WriteLine("Tentacle update failed");
                }
            }
            else 
            {
                Console.WriteLine("Tentacle is up to date");
            }
            
            if (!machine.HasLatestCalamari)
            {
                Console.WriteLine("Updating Calamari on target machine");
                var task = octopus.ExecuteCalamariUpdate(machine.Id);
                if (task.FinishedSuccessfully) 
                {
                    Console.WriteLine("Calamari update completed successfully");
                }
                else
                {
                    Console.WriteLine(task.ErrorMessage);
                    Console.WriteLine("Calamari update failed");
                }
            }
            else 
            {
                Console.WriteLine("Calamari is up to date");
            }
            
            return 0;
        }
    }
}