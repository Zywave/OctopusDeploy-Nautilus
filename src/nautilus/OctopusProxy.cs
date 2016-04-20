using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Nautilus
{
    internal class OctopusProxy
    {
        public OctopusProxy(IOctopusRepository octopusRepository)
        {
            if (octopusRepository == null) throw new ArgumentNullException(nameof(octopusRepository));

            Repository = octopusRepository;
        }
        
        private IOctopusRepository Repository { get; }
        
        public SystemInfoResource GetSystemInfo() 
        {
            var serverStatus = Repository.ServerStatus.GetServerStatus();
            return Repository.ServerStatus.GetSystemInfo(serverStatus);
        }
        
        public CertificateResource GetGlobalCertificate()
        {
            return Repository.Certificates.Get("certificate-global");
        }

        public MachineResource GetMachine(string name)
        {
            return Repository.Machines.FindByName(name);
        }
        
        public IEnumerable<MachineResource> GetMachines(string role)
        {
            return Repository.Machines.FindMany(m => m.Roles.Contains(role));
        }
                
        public MachineResource CreateMachine(string name, string thumbprint, string hostname, int port, IEnumerable<string> environmentNames, IEnumerable<string> roles)
        {
            var machine = new MachineResource();
            
            machine.Name = name;
            
            var environments = Repository.Environments.FindByNames(environmentNames);
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
                        
            return Repository.Machines.Create(machine);    
        }
        
        public MachineResource ModifyMachine(MachineResource machine, string thumbprint, string hostname, int port, IEnumerable<string> environmentNames, IEnumerable<string> roles)
        {            
            machine.EnvironmentIds.Clear();    
            var environments = Repository.Environments.FindByNames(environmentNames);
            foreach (var environment in environments)
            {                
                machine.EnvironmentIds.Add(environment.Id);
            }
           
            machine.Roles.Clear();
            foreach (var role in roles)
            {
                machine.Roles.Add(role);
            }
            
            var endpoint = new ListeningTentacleEndpointResource();
            endpoint.Uri = $"https://{hostname}:{port}";
            endpoint.Thumbprint = thumbprint;
            machine.Endpoint = endpoint;
                        
            return Repository.Machines.Modify(machine);
        }
        
        public void DeleteMachine(MachineResource machine)
        {
            Repository.Machines.Delete(machine);
        }
        
        public IEnumerable<ProjectResource> GetProjects()
        {
            return Repository.Projects.FindAll();
        }
        
        public IEnumerable<EnvironmentResource> GetEnvironments()
        {
            return Repository.Environments.FindAll();
        }
        
        public DeploymentProcessResource GetDeploymentProcess(string id)
        {
            return Repository.DeploymentProcesses.Get(id);
        }
        
        public DashboardResource GetDynamicDashboard(IEnumerable<string> projectIds, IEnumerable<string> environmentIds)
        {
            return Repository.Dashboards.GetDynamicDashboard(projectIds.ToArray(), environmentIds.ToArray());
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
            
            var deployment = Repository.Deployments.Create(deploymentResource);
            
            return deployment;
        }
        
        public TaskResource ExecuteTentacleUpgrade(string machineId)
        {            
            var task = Repository.Tasks.ExecuteTentacleUpgrade(null, null, new[] { machineId });
            Repository.Tasks.WaitForCompletion(task);
            task = Repository.Tasks.Get(task.Id);
            return task;
        }
        
        public TaskResource ExecuteCalamariUpdate(string machineId)
        {
            var task = Repository.Tasks.ExecuteCalamariUpdate(null, new[] { machineId });
            Repository.Tasks.WaitForCompletion(task);
            task = Repository.Tasks.Get(task.Id);
            return task;
        }
        
        public TaskResource WaitForTaskCompletion(string taskId)
        {
            var task = Repository.Tasks.Get(taskId);
            Repository.Tasks.WaitForCompletion(task);
            task = Repository.Tasks.Get(taskId);
            return task;
        }
        
        public TaskResource GetTask(string taskId)
        {
            return Repository.Tasks.Get(taskId);
        }
        
        public string GetTaskRawOutputLog(TaskResource task)
        {
            return Repository.Tasks.GetRawOutputLog(task);
        }
    }
}
