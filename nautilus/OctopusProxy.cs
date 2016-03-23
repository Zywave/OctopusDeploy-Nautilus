using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public IEnumerable<ProjectResource> GetProjects()
        {
            return _repository.Projects.FindAll();
        }
        
        public DeploymentProcessResource GetDeploymentProcess(string id)
        {
            return _repository.DeploymentProcesses.Get(id);
        }
        
        public DashboardResource GetDynamicDashboard(IEnumerable<string> projectIds, IEnumerable<string> environmentIds)
        {
            return _repository.Dashboards.GetDynamicDashboard(projectIds.ToArray(), environmentIds.ToArray());
        }
        
        public DeploymentResource CreateDeployment(string machineId, string releaseId, string environmentId, string comment)
        {
            var deploymentResource = new DeploymentResource
            {                
                ReleaseId = releaseId,
                EnvironmentId = environmentId,
                SpecificMachineIds = new ReferenceCollection(new[] { machineId }),
                Comments = comment
            };
            
            return _repository.Deployments.Create(deploymentResource);
        }

        private IOctopusRepository _repository;
    }
}
