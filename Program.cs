using System;
using System.Collections.Generic;
using CommandLine;

namespace Nautilus
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                var octopus = new OctopusProxy(options.OctopusServerAddress, options.OctopusApiKey);
                
                var machine = octopus.GetMachine(Environment.MachineName);
                
                var deployProjectIds = new List<string>();
                
                var projects = octopus.GetProjects();
                foreach(var project in projects) 
                {
                    var deploymentProcess = octopus.GetDeploymentProcess(project.DeploymentProcessId);
                    
                    if (deploymentProcess.HasAnyRole(machine.Roles))
                    {
                        
                        deployProjectIds.Add(project.Id);
                    }
                }
                
                var dashboard = octopus.GetDynamicDashboard(deployProjectIds, machine.EnvironmentIds);
                
                foreach(var item in dashboard.Items)
                {
                    var deployment = octopus.CreateDeployment(machine.Id, item.ReleaseId, item.EnvironmentId, $"Nautilus: {machine.Id}");
                }
            }

            Console.ReadLine();
        }
    }
}
