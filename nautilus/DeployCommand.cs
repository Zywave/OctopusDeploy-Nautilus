using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;

namespace Nautilus
{    
    [Verb("deploy", HelpText = "Creates deployments for the latest release of all projects related to the target machine by role and environment.")]
    public class DeployCommand : CommandBase
    {
        public override int Run()
        {
            var octopus = new OctopusProxy(OctopusServerAddress, OctopusApiKey);
                
            var machineName = MachineName ?? Environment.MachineName;
            
            Console.WriteLine($"Preparing to deploy to target machine ({machineName})...");
            
            var machine = octopus.GetMachine(machineName);
            if (machine == null)
            {
                Console.WriteLine($"Error: The target machine ({machineName}) is not registered with Octopus ({OctopusServerAddress}).");
                return 1;
            }
            
            Console.WriteLine($"{machine.Id} {machine.Name} {String.Join(",", machine.Roles)} {String.Join(",", machine.EnvironmentIds)}");
            
            Console.WriteLine($"Finding projects with the target roles ({String.Join(",", machine.Roles)})...");
            
            var projectIds = new List<string>();            
            var projects = octopus.GetProjects();
            foreach(var project in projects) 
            {
                var deploymentProcess = octopus.GetDeploymentProcess(project.DeploymentProcessId);
                
                if (deploymentProcess.HasAnyRole(machine.Roles))
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
            
            Console.WriteLine($"Creating deployments for target environments ({String.Join(",", machine.EnvironmentIds)})...");
            
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
    }
}