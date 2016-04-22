using System;
using Octopus.Client.Exceptions;

namespace Nautilus
{    
    public partial class NautilusService
    {   
        public void Unregister(string machineName = null)
        {
            try
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