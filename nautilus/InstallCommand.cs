using CommandLine;

namespace Nautilus
{    
    [Verb("install", HelpText = "Installs an Octopus tenticle on the target machine.")]
    public class InstallCommand : CommandBase
    {
        public override int Run()
        {            
            return 0;
        }
    }
}