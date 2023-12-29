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
        [Option(longName: "assembly", shortName: 'a', Required = true, 
                HelpText = "Name of the assembly (dll) in which the type is located")]
        public string AssemblyName { get; set; } = string.Empty;

        [Option(longName: "type", shortName: 't', Required = true,
                HelpText = "The type for which to generate a XSD schema")]
        public string Type { get; set; } = string.Empty;

        [Option(longName: "doc", shortName: 'd', Required = false,
                HelpText = "Path where the documentation for the type is located [ default, empty, which means no documentation will be included in the schema ]")]
        public string DocumentationDirectory { get; set; } = string.Empty;

        [Option(longName: "output-path", shortName: 'p', Required = false,
                HelpText = "Output path for the XSD schema, if omitted the XSD schema will be generated in the current working directory")]
        public string OutputPath { get; set; } = string.Empty;

        [Option(longName: "input-format", shortName: 'i', Required = false, 
                HelpText = "Input format for documentation files, valid values are txt, md, or html [ default: md ]")]
        public string InputFormat { get; set; } = "md";

        [Option(longName: "output-format", shortName: 'o', Required = false, 
                HelpText = "Output format for the xs:documentation elements, valid values are txt or html [ default: html ]")]
        public string OutputFormat { get; set; } = "html";

        [Option(longName: "encode", shortName: 'e', Required = false,
              HelpText = "Encode html tags xs:documentation text")]
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

                if (string.IsNullOrEmpty(OutputPath))
                {
                    File.WriteAllText(generator.FileName, content);
                }
                else
                {
                    if (!Directory.Exists(OutputPath)) 
                    {
                        throw new ArgumentException($"Output path [ {OutputPath} ] does not exists");
                    }

                    File.WriteAllText(Path.Combine(OutputPath, generator.FileName), content);
                }

                Console.WriteLine("done");
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }
        }
    }
}
