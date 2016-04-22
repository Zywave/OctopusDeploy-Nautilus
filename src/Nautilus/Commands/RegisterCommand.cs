#if CLI
using System.Collections.Generic;
using CommandLine;

namespace Nautilus.Commands
{    
    [Verb("register", HelpText = "Registers the local machine with Octopus.")]
    public class RegisterCommand : CommandBase
    {
        [Option('n', "name", Required = false, HelpText = "The machine name. Defaults to the local machine name.")]
        public string MachineName { get; set; }
        
        [Option('t', "thumbprint", Required = false, HelpText = "The Octopus Tentacle thumbprint. Defaults to the local Tentacle thumbprint.")]
        public string Thumbprint { get; set; }        
        
        [Option('h', "host", Required = false, HelpText = "The Tentacle host name. Defaults to the local machine name.")]
        public string HostName { get; set; }
        
        [Option('p', "port", Required = false, HelpText = "The Tentacle port. Defaults to 10933.")]
        public int? Port { get; set; }
        
        [Option('e', "environments", Required = true, HelpText = "The environment names of the machine.")]
        public IList<string> Environments { get; set; }
        
        [Option('r', "roles", Required = true, HelpText = "The roles of the machine.")]
        public IList<string> Roles { get; set; }
        
        [Option('u', "update", Required = false, HelpText = "Specifies whether to update an existing registration.")]
        public bool Update { get; set; }
        
        protected override void Run(INautilusService service)
        {
            service.Register(Environments, Roles, MachineName, Thumbprint, HostName, Port, Update);
        }
    }
}
#endif