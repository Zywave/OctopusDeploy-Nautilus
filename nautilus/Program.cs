using System.Net;
using CommandLine;

namespace Nautilus
{
    public class Program
    {
        public static int Main(string[] args)
        {            
            // Enable TLS 1.2
            ServicePointManager.SecurityProtocol =
               SecurityProtocolType.Ssl3
               | SecurityProtocolType.Tls
               | SecurityProtocolType.Tls11
               | SecurityProtocolType.Tls12;
            
            return CommandLine.Parser.Default.ParseArguments<DeployCommand, InstallCommand, UpdateCommand, RegisterCommand, UnregisterCommand>(args)
                .MapResult(
                    (DeployCommand command) => command.Run(),
                    (InstallCommand command) => command.Run(),
                    (UpdateCommand command) => command.Run(),
                    (RegisterCommand command) => command.Run(),
                    (UnregisterCommand command) => command.Run(),
                    errors => 1);
        }
    }
}
