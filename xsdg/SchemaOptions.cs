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
    public class SchemaOptions :
        Options
    {
        [Option(longName: "assembly", shortName: 'a', Required = true)]
        public string AssemblyName { get; set; } = string.Empty;

        [Option(longName: "type", shortName: 't', Required = true)]
        public string Type { get; set; } = string.Empty;

        [Option(longName: "doc", shortName: 'd', Required = false)]
        public string DocumentationDirectory { get; set; } = string.Empty;

        [Option(longName: "input-format", shortName: 'i', Required = false)]
        public string InputFormat { get; set; } = "md";

        [Option(longName: "output-format", shortName: 'o', Required = false)]
        public string OutputFormat { get; set; } = "html";

        [Option(longName: "encode", shortName: 'e', Required = false)]
        public bool EncodeHtml { get; set; } = true;

        private XSDGenerator GetGenerator(ObjectDocument document)
        {
            if (string.IsNullOrEmpty(DocumentationDirectory)) 
            {
                return new XSDGenerator(document);
            }
            else
            {
                var documentation = DocumentationSource.Create(document, DocumentationDirectory)
                    .SetInputFormat(GetFormat(InputFormat))
                    .SetOutputFormat(GetFormat(OutputFormat))
                    .SetEncoding(EncodeHtml)
                    .SetCharacterData(false)
                    .Build();

                return new XSDGenerator(document, documentation);
            }
        }

        public void Run()
        {
            try
            {
                var type = LoadType(AssemblyName, Type);
                var document = Inspector.Run(type.GetType());
                var generator = GetGenerator(document);

                Console.Write($"Generating XSD schema for [ {Type} ] ... ");
                var content = generator.Run();                

                File.WriteAllText(generator.FileName, content);
                Console.WriteLine("done");
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }
        }
    }
}
