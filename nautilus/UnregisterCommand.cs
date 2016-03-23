using CommandLine;

namespace Nautilus
{    
    [Verb("unregister", HelpText = "Unregisters the target machine from Octopus.")]
    public class UnregisterCommand : CommandBase
    {
        public override int Run()
        {            
            return 0;
        }
    }
}