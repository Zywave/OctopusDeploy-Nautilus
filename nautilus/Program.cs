using CommandLine;

namespace Nautilus
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return CommandLine.Parser.Default.ParseArguments<DeployCommand, InstallCommand, RegisterCommand, UnregisterCommand>(args)
                .MapResult(
                    (DeployCommand command) => command.Run(),
                    (InstallCommand command) => command.Run(),
                    (RegisterCommand command) => command.Run(),
                    (UnregisterCommand command) => command.Run(),
                    errors => 1);
        }
    }
}
