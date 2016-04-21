#if CLI
using CommandLine;

namespace Nautilus.Commands
{    
    [Verb("unregister", HelpText = "Unregisters the local machine from Octopus.")]
    public class UnregisterCommand : CommandBase
    {
        [Option('n', "name", Required = false, HelpText = "The machine name. Defaults to the local machine name.")]
        public string MachineName { get; set; }
        
        protected override int Run(INautilusService service)
        {            
            return service.Unregister(MachineName);
        }
    }
}
#endif