using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;

namespace Nautilus
{    
    public class NautilusService
    {
        public NautilusService(IOctopusRepository octopusRepository, TextWriter log = null)
        {
            if (octopusRepository == null) throw new ArgumentNullException(nameof(octopusRepository));
            
            _octopus = new OctopusProxy(octopusRepository);
            _log = log ?? TextWriter.Null;
        }
        
        public void Deploy(string machineName = null, bool wait = false, bool force = false, int? nonce = null)
        {
            var machineName = machineName ?? Environment.MachineName;
                            
            var machine = _octopus.GetMachine(machineName);
            if (machine == null)
            {
                _log.WriteLine($"Error: The target machine ({machineName}) is not registered with Octopus ({OctopusServerAddress})");
                return 1;
            }
            
            if (nonce.HasValue && CheckAndUpdateNonces(nonce.Value))
            {
                _log.WriteLine($"Preventing repeat deploy based on the specified nonce value ({nonce.Value})");
                return 0;
            }            
            
            var successExpression = new Regex($"Success: {machine.Name}{Environment.NewLine}");
            
            var environments = _octopus.GetEnvironments().ToDictionary(i => i.Id);
            
            _log.WriteLine($"Target machine: {machine.Id} {machine.Name} {String.Join(",", machine.Roles)} {String.Join(",", machine.EnvironmentIds.Select(e => GetEnvironmentName(e, environments)))}");
            
            _log.WriteLine($"Finding projects with the target roles ({String.Join(",", machine.Roles)})...");
            
            var matchedProjects = new Dictionary<string, ProjectResource>();            
            var projects = _octopus.GetProjects();
            foreach(var project in projects) 
            {
                var deploymentProcess = _octopus.GetDeploymentProcess(project.DeploymentProcessId);
                
                if (HasAnyRole(deploymentProcess, machine.Roles))
                {
                    matchedProjects[project.Id] = project;
                    _log.WriteLine(Indent($"{project.Id} {project.Name}"));
                }
            }
            
            if (!matchedProjects.Any())
            {
                _log.WriteLine(Indent("No projects found"));
                return 0;
            }
            
            var dashboard = octopus.GetDynamicDashboard(matchedProjects.Keys, machine.EnvironmentIds);
            
            WriteLine($"Creating deployments for target environments ({String.Join(",", machine.EnvironmentIds.Select(e => GetEnvironmentName(e, environments)))})...");
            
            foreach(var item in dashboard.Items)
            {
                if (item.ReleaseId != null)
                {
                    var project = matchedProjects[item.ProjectId];
                                        
                    _log.Write($"{project.Name} {item.ReleaseVersion} -> {GetEnvironmentName(item.EnvironmentId, environments)}... ", 1);
                    
                    if (!force)
                    {
                        var releaseTask = _octopus.GetTask(item.TaskId);
                        if (releaseTask.FinishedSuccessfully)
                        {
                            var rawOuput = _octopus.GetTaskRawOutputLog(releaseTask);
                            if (successExpression.IsMatch(rawOuput))
                            {
                                _log.WriteLine("already deployed");
                                continue;
                            }
                        }
                    }
                    
                    try
                    {
                        var deployment = _octopus.CreateDeployment(machine.Id, item.ReleaseId, item.EnvironmentId, $"Nautilus: {machine.Id}");
                        
                        if (wait) 
                        {
                            var task = _octopus.WaitForTaskCompletion(deployment.TaskId);
                            if (task.FinishedSuccessfully)
                            {
                                _log.WriteLine("succeeded");
                            }
                            else
                            {
                                _log.WriteLine("failed");
                                _log.WriteLine(Indent(task.ErrorMessage));
                            }
                            continue;                        
                        }
                            
                        _log.WriteLine("created");
                    }
                    catch (OctopusValidationException ex)
                    {
                        _log.WriteLine("invalid");
                        _log.WriteLine(Indent(ex.ToString()));
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
       
        private static string Indent(string value, Action<string> write, int indent = 1)
        {
            var indentString = new String(' ', indent);
            return indentString + value.Replace("\n", "\n" + indentString);
        }
        
        private readonly OctopusProxy _octopus;
        private readonly TextWriter _log;
    }
}