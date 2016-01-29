using CommandLine;
using CommandLine.Text;

namespace Nautilus
{
    public class Options
    {
        [Option("server", Required = true, HelpText = "Octopus server address (e.g. http://your-octopus-server/)".)]
        public string OctopusServerAddress { get; set; }

        [Option("apiKey", Required = true, HelpText = "Octopus API key.")]
        public string OctopusApiKey { get; set; }
        
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this);
        }
    }
}
