using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

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
        
        public SystemInfoResource GetSystemInfo() 
        {
            var serverStatus = _repository.ServerStatus.GetServerStatus();
            return _repository.ServerStatus.GetSystemInfo(serverStatus);
        }
        
        public CertificateResource GetGlobalCertificate()
        {
            return _repository.Certificates.Get("certificate-global");
        }

        public MachineResource GetMachine(string name)
        {
            return _repository.Machines.FindByName(name);
        }
        
        public MachineResource CreateMachine(string name, string thumbprint, string hostname, int port, IEnumerable<string> environmentNames, IEnumerable<string> roles)
        {
            var machine = new MachineResource();
            
            machine.Name = name;
            
            var environments = _repository.Environments.FindByNames(environmentNames);
            foreach (var environment in environments)
            {                
                machine.EnvironmentIds.Add(environment.Id);
            }
           
            foreach (var role in roles) 
            {
                machine.Roles.Add(role);
            }
            
            var endpoint = new ListeningTentacleEndpointResource();
            endpoint.Uri = $"https://{hostname}:{port}";
            endpoint.Thumbprint = thumbprint;
            machine.Endpoint = endpoint;
                        
            return _repository.Machines.Create(machine);    
        }
        
        public void DeleteMachine(MachineResource machine)
        {
            _repository.Machines.Delete(machine);
        }
        
        public IEnumerable<ProjectResource> GetProjects()
        {
            return _repository.Projects.FindAll();
        }
        
        public IEnumerable<EnvironmentResource> GetEnvironments()
        {
            return _repository.Environments.FindAll();
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
            
            var deployment = _repository.Deployments.Create(deploymentResource);
            
            return deployment;
        }
        
        public TaskResource ExecuteTentacleUpgrade(string machineId)
        {            
            var task = _repository.Tasks.ExecuteTentacleUpgrade(null, null, new[] { machineId });
            _repository.Tasks.WaitForCompletion(task);
            task = _repository.Tasks.Get(task.Id);
            return task;
        }
        
        public TaskResource ExecuteCalamariUpdate(string machineId)
        {
            var task = _repository.Tasks.ExecuteCalamariUpdate(null, new[] { machineId });
            _repository.Tasks.WaitForCompletion(task);
            task = _repository.Tasks.Get(task.Id);
            return task;
        }
        
        public TaskResource WaitForTaskCompletion(string taskId)
        {
            var task = _repository.Tasks.Get(taskId);
            _repository.Tasks.WaitForCompletion(task);
            task = _repository.Tasks.Get(taskId);
            return task;
        }

        private IOctopusRepository _repository;
    }
}
