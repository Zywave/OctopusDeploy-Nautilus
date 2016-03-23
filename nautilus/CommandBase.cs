using CommandLine;

namespace Nautilus
{
    public abstract class CommandBase
    {        
        [Option("server", Required = true, HelpText = "Octopus server address (e.g. http://your-octopus-server/).")]
        public string OctopusServerAddress { get; set; }

        [Option("apiKey", Required = true, HelpText = "Octopus API key.")]
        public string OctopusApiKey { get; set; }
        
        [Option("machine", Required = false, HelpText = "The target machine name.")]
        public string MachineName { get; set; }
        
        public abstract int Run();
    }
}
