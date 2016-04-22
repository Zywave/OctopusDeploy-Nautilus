#if CLI
using System;
using CommandLine;
using Octopus.Client;

namespace Nautilus.Commands
{
    public abstract class CommandBase 
    {
        [Option('s', "server", Required = true, HelpText = "Octopus server address (e.g. http://your-octopus-server/).")]
        public string OctopusServerAddress { get; set; }

        [Option('k', "apikey", Required = true, HelpText = "Octopus API key.")]
        public string OctopusApiKey { get; set; }

        public int Run()
        {
            var octopusRepository = new OctopusRepository(new OctopusServerEndpoint(OctopusServerAddress, OctopusApiKey));
            var nautilusService = new NautilusService(octopusRepository, Console.Out);
            
            try 
            {
                Run(nautilusService);
            }
            catch (NautilusException ex)
            {
                Console.Error.WriteLine(ex.Message);
                if (!String.IsNullOrEmpty(ex.MessageDetails))
                {
                    Console.Error.WriteLine(ex.MessageDetails);
                }
                if (ex.InnerException != null)
                {
                    Console.Error.WriteLine(ex.InnerException.ToString());
                }
                
                return ex.ErrorCode;
            }
            
            return 0;
        }

        protected abstract void Run(INautilusService service);        
    }    
}
#endif