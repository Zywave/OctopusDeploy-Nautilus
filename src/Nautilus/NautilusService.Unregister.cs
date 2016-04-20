using System;

namespace Nautilus
{    
    public partial class NautilusService
    {   
        public int Unregister(string machineName = null)
        {
            machineName = machineName ?? Environment.MachineName;
            
            var machine = Octopus.GetMachine(machineName);
            if (machine == null)
            {
                Log.WriteLine($"The machine ({machineName}) is not registered with Octopus");
                return 0;
            }
            
            Octopus.DeleteMachine(machine);
            
            Log.WriteLine($"The machine ({machine.Name}) was unregistered successfully");
            
            return 0;
        }
    }
}