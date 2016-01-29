using System;
using CommandLine;

namespace Nautilus
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {

            }

            Console.ReadLine();
        }
    }
}
