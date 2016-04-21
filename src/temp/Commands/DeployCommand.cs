#if CLI
using CommandLine;

namespace Nautilus.Commands
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
        
        protected override int Run(INautilusService service)
        {
            return service.Deploy(MachineName, Wait, Force, Nonce);
        }
    }
}
#endif