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
        
        [Option('w', "wait", Required = false, HelpText = "Specifies whether to wait for each deployment to complete before exiting.")]
        public bool Wait { get; set; }
        
        protected override int Run(OctopusProxy octopus)
        {                
            var machineName = MachineName ?? Environment.MachineName;
                        
            var machine = octopus.GetMachine(machineName);
            if (machine == null)
            {
                WriteLine($"Error: The target machine ({machineName}) is not registered with Octopus ({OctopusServerAddress})");
                return 1;
            }
            
            var environments = octopus.GetEnvironments().ToDictionary(i => i.Id);
            
            WriteLine($"Target machine: {machine.Id} {machine.Name} {String.Join(",", machine.Roles)} {String.Join(",", machine.EnvironmentIds.Select(e => GetEnvironmentName(e, environments)))}");
            
            WriteLine($"Finding projects with the target roles ({String.Join(",", machine.Roles)})...");
            
            var matchedProjects = new Dictionary<string, ProjectResource>();            
            var projects = octopus.GetProjects();
            foreach(var project in projects) 
            {
                var deploymentProcess = octopus.GetDeploymentProcess(project.DeploymentProcessId);
                
                if (HasAnyRole(deploymentProcess, machine.Roles))
                {
                    matchedProjects[project.Id] = project;
                    WriteLine($" {project.Id} {project.Name}");
                }
            }
            
            if (!matchedProjects.Any())
            {
                WriteLine(" No projects found");
                return 0;
            }
            
            var dashboard = octopus.GetDynamicDashboard(matchedProjects.Keys, machine.EnvironmentIds);
            
            WriteLine($"Creating deployments for target environments ({String.Join(",", machine.EnvironmentIds.Select(e => GetEnvironmentName(e, environments)))})...");
            
            foreach(var item in dashboard.Items)
            {
                if (item.ReleaseId != null)
                {
                    var project = matchedProjects[item.ProjectId];
                    var deployment = octopus.CreateDeployment(machine.Id, item.ReleaseId, item.EnvironmentId, $"Nautilus: {machine.Id}");
                    Write($" {deployment.Id} {project.Name} {item.ReleaseVersion} -> {GetEnvironmentName(item.EnvironmentId, environments)}");
                    if (Wait) 
                    {
                        Write($"... ");
                        var task = octopus.WaitForTaskCompletion(deployment.TaskId);
                        WriteLine(task.FinishedSuccessfully ? "succeeded" : "failed", task.FinishedSuccessfully ? ConsoleColor.Green : ConsoleColor.Red);                        
                    }
                    else
                    {
                        WriteLine();
                    }
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
        
        private static string GetEnvironmentName(string environmentId, IDictionary<string, EnvironmentResource> lookup)
        {
            EnvironmentResource environment;
            if (lookup.TryGetValue(environmentId, out environment))
            {
                return environment.Name;
            }
            return environmentId;
        }
    }
}