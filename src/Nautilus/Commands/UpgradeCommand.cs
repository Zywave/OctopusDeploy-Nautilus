#if CLI
using CommandLine;

namespace Nautilus.Commands
{
    [Verb("upgrade", HelpText = "Upgrades the Octopus Tentacle on the target machine.")]
    public class UpgradeCommand : CommandBase
    {
        [Option('n', "name", Required = false, HelpText = "The target machine name. Defaults to the local machine name.")]
        public string MachineName { get; set; }
        
        protected override void Run(INautilusService service)
        {                
            service.Upgrade(MachineName);
        }
    }
}
#endif