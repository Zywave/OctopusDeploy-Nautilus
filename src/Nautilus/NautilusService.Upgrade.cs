using System;
using Octopus.Client.Model.Endpoints;

namespace Nautilus
{    
    public partial class NautilusService
    {   
        public int Upgrade(string machineName = null)
        {
            machineName = machineName ?? Environment.MachineName;
                        
            var machine = Octopus.GetMachine(machineName);
            if (machine == null)
            {
                Log.WriteLine($"Error: The target machine ({machineName}) is not registered with Octopus");
                return 1;
            }
            
            var tentacleVersionDetails = (machine.Endpoint as ListeningTentacleEndpointResource)?.TentacleVersionDetails;            
            if (tentacleVersionDetails != null && !tentacleVersionDetails.UpgradeLocked && (tentacleVersionDetails.UpgradeSuggested || tentacleVersionDetails.UpgradeRequired))
            {
                Log.Write("Updating Tentacle on target machine... ");
                var task = Octopus.ExecuteTentacleUpgrade(machine.Id);
                if (task.FinishedSuccessfully) 
                {
                    Log.WriteLine("succeeded");
                }
                else
                {
                    Log.WriteLine("failed");
                    Log.WriteLine(task.ErrorMessage);
                }
            }
            else 
            {
                Log.WriteLine("Tentacle is up to date");
            }
            
            if (!machine.HasLatestCalamari)
            {
                Log.Write("Updating Calamari on target machine... ");
                var task = Octopus.ExecuteCalamariUpdate(machine.Id);
                if (task.FinishedSuccessfully) 
                {
                    Log.WriteLine("succeeded");
                }
                else
                {
                    Log.WriteLine("failed");
                    Log.WriteLine(task.ErrorMessage);
                }
            }
            else 
            {
                Log.WriteLine("Calamari is up to date");
            }
            
            return 0;
        }
    }
}