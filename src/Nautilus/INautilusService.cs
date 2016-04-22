using System.Collections.Generic;

namespace Nautilus
{    
    public interface INautilusService
    {        
        void Deploy(string machineName = null, bool wait = false, bool force = false, int? nonce = null);
        void Install(string installLocation = null, string homeLocation = null, string appLocation = null, string thumbprint = null, int? port = null);
        void Register(IEnumerable<string> environments, IEnumerable<string> roles, string machineName = null, string thumbprint = null, string hostName = null, int? port = null, bool update = false);
        void Unregister(string machineName = null);
        void Upgrade(string machineName = null);
        void Purge(string role);
    }
}