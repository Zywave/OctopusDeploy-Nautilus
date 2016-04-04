using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Octopus.Client.Model;

namespace Nautilus
{    
    [Verb("deploy", HelpText = "Creates deployments for the latest release of all projects related to the target machine by role and environment.")]
    public class DeployCommand : CommandBase
    {
        [Option('n', "name", Required = false, HelpText = "The target machine name. Defaults to the local machine name.")]
        public string MachineName { get; set; }
        
        protected override int Run(OctopusProxy octopus)
        {                
            var machineName = MachineName ?? Environment.MachineName;
            
            Console.WriteLine($"Preparing to deploy to target machine ({machineName})");
            
            var machine = octopus.GetMachine(machineName);
            if (machine == null)
            {
                Console.WriteLine($"Error: The target machine ({machineName}) is not registered with Octopus ({OctopusServerAddress})");
                return 1;
            }
            
            Console.WriteLine($"{machine.Id} {machine.Name} {String.Join(",", machine.Roles)} {String.Join(",", machine.EnvironmentIds)}");
            
            Console.WriteLine($"Finding projects with the target roles ({String.Join(",", machine.Roles)})");
            
            var projectIds = new List<string>();            
            var projects = octopus.GetProjects();
            foreach(var project in projects) 
            {
                var deploymentProcess = octopus.GetDeploymentProcess(project.DeploymentProcessId);
                
                if (HasAnyRole(deploymentProcess, machine.Roles))
                {
                    projectIds.Add(project.Id);
                    Console.WriteLine($"{project.Id} {project.Name}");
                }
            }
            
            if (!projectIds.Any())
            {
                Console.WriteLine("No projects found");
                return 0;
            }
            
            var dashboard = octopus.GetDynamicDashboard(projectIds, machine.EnvironmentIds);
            
            Console.WriteLine($"Creating deployments for target environments ({String.Join(",", machine.EnvironmentIds)})");
            
            foreach(var item in dashboard.Items)
            {
                if (item.ReleaseId != null)
                {
                    var deployment = octopus.CreateDeployment(machine.Id, item.ReleaseId, item.EnvironmentId, $"Nautilus: {machine.Id}");
                    Console.WriteLine($"{deployment.Id} {item.ReleaseVersion} {item.EnvironmentId}");
                }
            }
            
            return 0;
        }
        
        private static bool HasAnyRole(DeploymentProcessResource deploymentProcess, IEnumerable<string> roles)
        {
            foreach (var step in deploymentProcess.Steps) 
            {
                PropertyValueResource targetRolesProperty;
                if (step.Properties.TryGetValue("Octopus.Action.TargetRoles", out targetRolesProperty))
                {
                    var targetRoles = targetRolesProperty.Value.Split(',');
                    if (roles.Intersect(targetRoles).Any())
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
    }
}