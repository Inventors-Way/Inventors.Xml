using CommandLine;
using Inventors.Xml;
using Inventors.Xml.Serialization;
using Inventors.Xml.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace xsdg
{
    public class Options
    {
        [Option(longName: "path", shortName: 'p', Required = false,
                HelpText = "The working path")]
        public string Path { get; set; } = string.Empty;

        [Value(0, Required = true, HelpText = "Configuration file")]
        public string ConfigFile { get; set; } = string.Empty;

        public void Run()
        {
            var xsdSchema = GetType().Assembly.ReadEmbeddedResourceString("Schema.xsdg.xsd");

            if (!File.Exists(ConfigFile))
            {
                Console.WriteLine($"error, file not found!");
                return;
            }

            if (string.IsNullOrEmpty(Path))
            {
                Path = Directory.GetCurrentDirectory();
            }

            if (!Directory.Exists(Path))
            {
                Console.WriteLine($"Working path [ {Path} ] does not exists, aborting");
                return;
            }

            Console.WriteLine($"Working path set to: {Path}");

            Console.Write($"Loading configuration file [ {ConfigFile} ] ... ");


            var text = File.ReadAllText(ConfigFile);
            text.ToObject<XSDGConfig>(xsdSchema)
                .OnSuccess(config =>
                {
                    Console.WriteLine("done");
                    config.Run(Path);
                })
                .OnError(errors =>
                {
                    Console.WriteLine(("failed!"));
                    Console.WriteLine($"{errors}");
                });            
        }
    }
}
