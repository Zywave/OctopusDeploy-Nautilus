using System;
using Octopus.Client;
using Octopus.Client.Model;

namespace Nautilus
{
    public class OctopusProxy
    {
        public OctopusProxy(string serverAddress, string apiKey)
        {
            if (serverAddress == null) throw new ArgumentNullException(nameof(serverAddress));
            if (apiKey == null) throw new ArgumentNullException(nameof(apiKey));

            _repository = new OctopusRepository(new OctopusServerEndpoint(serverAddress, apiKey));
        }

        public MachineResource GetMachine(string name)
        {
            return _repository.Machines.FindOne(m => m.Name == name);
        }

        private IOctopusRepository _repository;
    }
}
