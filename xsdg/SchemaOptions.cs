using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xsdg
{
    [Verb("xsd", HelpText = "Generate XSD schema")]
    public class SchemaOptions
    {
        [Option(longName: "assembly", shortName: 'a', Required = true)]
        public string AssemblyName { get; set; } = string.Empty;

        [Option(longName: "type", shortName: 't', Required = true)]
        public string Type { get; set; } = string.Empty;

        public void Run()
        {

        }
    }
}
