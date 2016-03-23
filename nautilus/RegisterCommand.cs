using CommandLine;

namespace Nautilus
{    
    [Verb("register", HelpText = "Registers the target machine with Octopus.")]
    public class RegisterCommand : CommandBase
    {
        public override int Run()
        {            
            return 0;
        }
    }
}