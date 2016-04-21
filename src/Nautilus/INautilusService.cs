using System.Collections.Generic;

namespace Nautilus
{    
    public interface INautilusService
    {        
        int Deploy(string machineName = null, bool wait = false, bool force = false, int? nonce = null);
        int Install(string installLocation = null, string homeLocation = null, string appLocation = null, string thumbprint = null, int? port = null);
        int Register(IEnumerable<string> environments, IEnumerable<string> roles, string machineName = null, string thumbprint = null, string hostName = null, int? port = null, bool update = false);
        int Unregister(string machineName = null);
        int Upgrade(string machineName = null);
        int Purge(string role);
    }
}