using System;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;

namespace Nautilus
{    
    public partial class NautilusService
    {   
        public void Purge(string role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            
            try
            {
                var machines = Octopus.GetMachines(role);
                foreach (var machine in machines)
                {
                    if (machine.Status == MachineModelStatus.Offline)
                    {
                        Octopus.DeleteMachine(machine);
                        
                        Log.WriteLine($"{machine.Name} is offline and was unregistered successfully");
                    }
                }
            }
            catch (NautilusException)
            {
                throw;
            }
            catch (OctopusException ex)
            {
                throw NautilusException.OctopusError(ex);
            }
            catch (Exception ex)
            {
                throw NautilusException.UnknownError(ex);
            } 
        }
    }
}