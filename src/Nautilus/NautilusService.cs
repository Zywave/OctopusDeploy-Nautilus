using System;
using System.IO;
using Octopus.Client;

namespace Nautilus
{    
    public partial class NautilusService : INautilusService
    {
        public NautilusService(IOctopusRepository octopusRepository, TextWriter log = null)
        {
            if (octopusRepository == null) throw new ArgumentNullException(nameof(octopusRepository));
            
            Octopus = new OctopusProxy(octopusRepository);
            Log = log ?? TextWriter.Null;
        }
        
        private OctopusProxy Octopus { get; }
        private TextWriter Log { get; }
       
        private static string Indent(string value, int indent = 1)
        {
            var indentString = new String(' ', indent);
            return indentString + value.Replace("\n", "\n" + indentString);
        }
    }
}