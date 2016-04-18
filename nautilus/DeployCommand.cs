using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;
using Octopus.Client.Exceptions;
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
        
        [Option('f', "force", Required = false, HelpText = "Specifies whether to force redeployment of releases to the target machine.")]
        public bool Force { get; set; }
        
        [Option('o', "nonce", Required = false, HelpText = "An arbritrary value to ensure that a deploy is only run once.  If the specified value matches a value previously used, this deploy will be prevented. The value is stored in an environment variable (NAUTILUS_NONCE) on the local machine.")]
        public int? Nonce { get; set; }
        
        protected override int Run(OctopusProxy octopus)
        {                
            var machineName = MachineName ?? Environment.MachineName;
                        
            var machine = octopus.GetMachine(machineName);
            if (machine == null)
            {
                WriteLine($"Error: The target machine ({machineName}) is not registered with Octopus ({OctopusServerAddress})");
                return 1;
            }
            
            if (Nonce.HasValue && CheckAndUpdateNonces(Nonce.Value))
            {
                WriteLine($"Preventing repeat deploy based on the specified nonce value ({Nonce.Value})");
                return 0;
            }            
            
            var successExpression = new Regex($"Success: {machine.Name}{Environment.NewLine}");
            
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
                    WriteLine($"{project.Id} {project.Name}", indent: 1);
                }
            }
            
            if (!matchedProjects.Any())
            {
                WriteLine("No projects found", indent: 1);
                return 0;
            }
            
            var dashboard = octopus.GetDynamicDashboard(matchedProjects.Keys, machine.EnvironmentIds);
            
            WriteLine($"Creating deployments for target environments ({String.Join(",", machine.EnvironmentIds.Select(e => GetEnvironmentName(e, environments)))})...");
            
            foreach(var item in dashboard.Items)
            {
                if (item.ReleaseId != null)
                {
                    var project = matchedProjects[item.ProjectId];
                                      
                    Write($"{project.Name} {item.ReleaseVersion} -> {GetEnvironmentName(item.EnvironmentId, environments)}... ", indent: 1);
                   
                    if (!Force)
                    {
                        var releaseTask = octopus.GetTask(item.TaskId);
                        if (releaseTask.FinishedSuccessfully)
                        {
                            var rawOuput = octopus.GetTaskRawOutputLog(releaseTask);
                            if (successExpression.IsMatch(rawOuput))
                            {
                                WriteLine("already deployed");
                                continue;
                            }
                        }
                    }
                    
                    try
                    {
                        var deployment = octopus.CreateDeployment(machine.Id, item.ReleaseId, item.EnvironmentId, $"Nautilus: {machine.Id}");
                        
                        if (Wait) 
                        {
                            var task = octopus.WaitForTaskCompletion(deployment.TaskId);
                            if (task.FinishedSuccessfully)
                            {
                                WriteLine("succeeded", ConsoleColor.Green);
                            }
                            else
                            {
                                WriteLine("failed", ConsoleColor.Red);
                                WriteLine(task.ErrorMessage, ConsoleColor.DarkGray, 1);
                            }
                            continue;                        
                        }
                            
                        WriteLine("created");
                    }
                    catch (OctopusValidationException ex)
                    {
                        WriteLine("invalid", ConsoleColor.Red);
                        WriteLine(ex.ToString(), ConsoleColor.DarkGray, 1);
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
        
        private static bool CheckAndUpdateNonces(int nonce)
        {
            var nonces = new HashSet<int>();
            var ev = Environment.GetEnvironmentVariable("NAUTILUS_NONCE", EnvironmentVariableTarget.Machine);
            if (!String.IsNullOrEmpty(ev))
            {
                foreach (var ns in ev.Split(','))
                {
                    int n;
                    if (int.TryParse(ns, out n))
                    {
                        nonces.Add(n);
                    }
                }
            }
            
            if (nonces.Contains(nonce))
            {
                return true;
            }            
            
            nonces.Add(nonce);
            Environment.SetEnvironmentVariable("NAUTILUS_NONCE", String.Join(",", nonces), EnvironmentVariableTarget.Machine);
            
            return false;
        }
    }
}
