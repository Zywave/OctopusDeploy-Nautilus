using System;

namespace Nautilus
{    
    public partial class NautilusService
    {   
        public void Unregister(string machineName = null)
        {
            machineName = machineName ?? Environment.MachineName;
            
            var machine = Octopus.GetMachine(machineName);
            if (machine == null)
            {
                Log.WriteLine($"The machine ({machineName}) is not registered with Octopus");
            }
            
            Octopus.DeleteMachine(machine);
            
            Log.WriteLine($"The machine ({machine.Name}) was unregistered successfully");
        }
    }
}