using CommandLine;
using Inventors.Xml.Content;
using Inventors.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Inventors.Xml.Generators.Xsd;
using System.Reflection.Metadata;

namespace xsdg
{
    [Verb("xsd", HelpText = "Generate XSD schema")]
    public class SchemaOptions
    {
        [Option(longName: "assembly", shortName: 'a', Required = true)]
        public string AssemblyName { get; set; } = string.Empty;

        [Option(longName: "type", shortName: 't', Required = true)]
        public string Type { get; set; } = string.Empty;

        [Option(longName: "doc", shortName: 'd', Required = false)]
        public string DocumentationDirectory { get; set; } = string.Empty;

        private XSDGenerator GetGenerator(ObjectDocument document)
        {
            if (string.IsNullOrEmpty(DocumentationDirectory)) 
            {
                return new XSDGenerator(document);
            }
            else
            {
                var documentation = DocumentationSource.Create(document, DocumentationDirectory)
                    .SetInputFormat(DocumentationFormat.MarkDown)
                    .SetOutputFormat(DocumentationFormat.Html)
                    .SetEncoding(true)
                .Build();

                return new XSDGenerator(document, documentation);
            }
        }

        public void Run()
        {
            if (!File.Exists(AssemblyName)) 
            {
                Console.WriteLine($"Did not assembly: {AssemblyName}");
            }

            try
            {
                var assembly = Assembly.LoadFrom(AssemblyName);
                var type = assembly.GetType(Type);
                var document = Inspector.Run(type.GetType());
                var generator = GetGenerator(document);

                Console.Write($"Generating XSD schema for [ {Type} ] ... ");
                var content = generator.Run();                

                File.WriteAllText(generator.FileName, content);
                Console.WriteLine("done");
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Exception while generating schema: {ex}");
            }
        }
    }
}
