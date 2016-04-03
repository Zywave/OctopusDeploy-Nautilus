using CommandLine;

namespace Nautilus
{
    public abstract class CommandBase
    {
        [Option('s', "server", Required = true, HelpText = "Octopus server address (e.g. http://your-octopus-server/).")]
        public string OctopusServerAddress { get; set; }

        [Option('k', "apikey", Required = true, HelpText = "Octopus API key.")]
        public string OctopusApiKey { get; set; }
                
        public int Run()
        {            
            var octopus = new OctopusProxy(OctopusServerAddress, OctopusApiKey);
            return Run(octopus);
        }
        
        protected abstract int Run(OctopusProxy octopus);
    }
}
