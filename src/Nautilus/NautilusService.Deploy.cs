using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;

namespace Nautilus
{    
    public partial class NautilusService
    {   
        public void Deploy(string machineName = null, bool wait = false, bool force = false, int? nonce = null)
        {
            machineName = machineName ?? Environment.MachineName;
                            
            var machine = Octopus.GetMachine(machineName);
            if (machine == null)
            {
                var message = $"The target machine ({machineName}) is not registered with Octopus";
                Log.WriteLine($"Error: {message}");
                throw new NautilusException(NautilusErrorCodes.MachineNotRegistered, message);
            }
            
            if (nonce.HasValue && CheckAndUpdateNonces(nonce.Value))
            {
                Log.WriteLine($"Preventing repeat deploy based on the specified nonce value ({nonce.Value})");
                return;
            }            
                        
            var environments = Octopus.GetEnvironments().ToDictionary(i => i.Id);
            
            Log.WriteLine($"Target machine: {machine.Id} {machine.Name} {String.Join(",", machine.Roles)} {String.Join(",", machine.EnvironmentIds.Select(e => GetEnvironmentName(e, environments)))}");
            
            Log.WriteLine($"Finding projects with the target roles ({String.Join(",", machine.Roles)})...");
            
            var matchedProjects = new Dictionary<string, ProjectResource>();            
            var projects = Octopus.GetProjects();
            foreach(var project in projects) 
            {
                var deploymentProcess = Octopus.GetDeploymentProcess(project.DeploymentProcessId);
                
                if (HasAnyRole(deploymentProcess, machine.Roles))
                {
                    matchedProjects[project.Id] = project;
                    Log.WriteLine(Indent($"{project.Id} {project.Name}"));
                }
            }
            
            if (!matchedProjects.Any())
            {
                Log.WriteLine(Indent("No projects found"));
                return;
            }
            
            var dashboard = Octopus.GetDynamicDashboard(matchedProjects.Keys, machine.EnvironmentIds);
            
            Log.WriteLine($"Creating deployments for target environments ({String.Join(",", machine.EnvironmentIds.Select(e => GetEnvironmentName(e, environments)))})...");
            
            var successExpression = new Regex($"Success: {machine.Name}{Environment.NewLine}");
            
            foreach(var item in dashboard.Items)
            {
                if (item.ReleaseId != null)
                {
                    var project = matchedProjects[item.ProjectId];
                                        
                    Log.Write($"{project.Name} {item.ReleaseVersion} -> {GetEnvironmentName(item.EnvironmentId, environments)}... ", 1);
                    
                    if (!force)
                    {
                        var releaseTask = Octopus.GetTask(item.TaskId);
                        if (releaseTask.FinishedSuccessfully)
                        {
                            var rawOuput = Octopus.GetTaskRawOutputLog(releaseTask);
                            if (successExpression.IsMatch(rawOuput))
                            {
                                Log.WriteLine("already deployed");
                                continue;
                            }
                        }
                    }
                    
                    try
                    {
                        var deployment = Octopus.CreateDeployment(machine.Id, item.ReleaseId, item.EnvironmentId, $"Nautilus: {machine.Id}");
                        
                        if (wait) 
                        {
                            var task = Octopus.WaitForTaskCompletion(deployment.TaskId);
                            if (task.FinishedSuccessfully)
                            {
                                Log.WriteLine("succeeded");
                            }
                            else
                            {
                                Log.WriteLine("failed");
                                Log.WriteLine(Indent(task.ErrorMessage));
                            }
                            continue;                        
                        }
                            
                        Log.WriteLine("created");
                    }
                    catch (OctopusValidationException ex)
                    {
                        Log.WriteLine("invalid");
                        Log.WriteLine(Indent(ex.ErrorMessage));
                    }
                }
            }
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