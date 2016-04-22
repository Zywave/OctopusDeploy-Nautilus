#if CLI
using CommandLine;

namespace Nautilus.Commands
{    
    [Verb("purge", HelpText = "Unregisters offline machines in a specified role.")]
    public class PurgeCommand : CommandBase
    {
        [Option('r', "role", Required = true, HelpText = "The machine role for which to purge offline nodes.")]
        public string Role { get; set; }
        
        protected override void Run(INautilusService service)
        {   
            service.Purge(Role);
        }
    }
}
#endif