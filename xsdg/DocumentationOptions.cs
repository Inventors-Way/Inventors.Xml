using CommandLine;
using Inventors.Xml.Content;
using Inventors.Xml.Generators.Xsd;
using Inventors.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventors.Xml.Generators.Documentation;

namespace xsdg
{
    [Verb("doc", HelpText = "Generate documentation files")]
    public class DocumentationOptions : Options
    {
        [Option(longName: "assembly", shortName: 'a', Required = true,
               HelpText = "Name of the assembly (dll) in which the type is located")]
        public string AssemblyName { get; set; } = string.Empty;

        [Option(longName: "type", shortName: 't', Required = true,
                HelpText = "The type for which to generate a XSD schema")]
        public string Type { get; set; } = string.Empty;

        [Option(longName: "doc", shortName: 'd', Required = true,
                HelpText = "Path where the documentation for the type will be generated")]
        public string DocumentationDirectory { get; set; } = string.Empty;

        [Option(longName: "input-format", shortName: 'i', Required = false,
                HelpText = "Format for documentation files to be generated, valid values are txt, md, or html [ default: md ]")]
        public string InputFormat { get; set; } = "md";

        private DocumentationGenerator GetGenerator(ObjectDocument document)
        {
            var documentation = DocumentationSource.Create(document, DocumentationDirectory)
                .SetInputFormat(GetFormat(InputFormat))
                .SetOutputFormat(DocumentationFormat.Text)
                .Build();

            return new DocumentationGenerator(document, documentation);
        }

        public void Run()
        {
            try
            {
                var type = LoadType(AssemblyName, Type);
                var document = Inspector.Run(type.GetType());
                var generator = GetGenerator(document);

                Console.Write($"Generating documentation templates for [ {Type} ]:");
                generator.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
