#if CLI
using CommandLine;

namespace Nautilus.Commands
{    
    [Verb("install", HelpText = "Installs an Octopus Tentacle on the local machine.")]
    public class InstallCommand : CommandBase
    {
        [Option('l', "location", Required = false, HelpText = "The install directory of the Octopus Tentacle. Defaults to \"%PROGRAMFILES%\\Octopus Deploy\\Tentacle\".")]
        public string InstallLocation { get; set; }
        
        [Option('h', "home", Required = false, HelpText = "The home directory of the Octopus Tentacle. Defaults to \"%SYSTEMDRIVE%\\Octopus\".")]
        public string HomeLocation { get; set; }
        
        [Option('a', "app", Required = false, HelpText = "The application directory of the Octopus Tentacle. Defaults to \"<home>\\Applications\".")]
        public string AppLocation { get; set; }
        
        [Option('t', "thumbprint", Required = false, HelpText = "The Octopus Server thumbprint. Defaults to global certificate thumbprint.")]
        public string Thumbprint { get; set; }      
               
        [Option('p', "port", Required = false, HelpText = "The port of the Octopus Tentacle. Defaults to 10933.")]
        public int? Port { get; set; }
        
        protected override int Run(INautilusService service)
        {            
            return service.Install(InstallLocation, HomeLocation, AppLocation, Thumbprint, Port);
        }
    }
}
#endif