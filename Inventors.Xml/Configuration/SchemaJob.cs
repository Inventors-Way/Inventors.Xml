using Inventors.Xml.Content;
using Inventors.Xml.Generators.Documentation;
using Inventors.Xml.Generators.Xsd;
using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Configuration
{
    public class SchemaJob :
        Job
    {
        [XmlAttribute("include-documentation")]
        [XmlRequired(false)]
        public bool IncludeDocumentation { get; set; } = true;

        [XmlAttribute("documentation-file-format")]
        [XmlRequired(false)]
        public DocumentationFormat DocumentationFileFormat { get; set; } = DocumentationFormat.MarkDown;

        [XmlAttribute("documentation-output-format")]
        [XmlRequired(false)]
        public DocumentationFormat DocumentationOutputFormat { get; set; } = DocumentationFormat.Html;

        [XmlAttribute("encode-data")]
        [XmlRequired(false)]
        public bool EncodeData { get; set; } = true;

        [XmlAttribute("encapsulate-character-data")]
        [XmlRequired(false)]    
        public bool EncapsulateCharacterData { get; set; } = false;

        public override void Run(string path, IJobConfiguration configuration)
        {
            var type = $"Loading type: {Type}".Run(() => LoadType(configuration));
            "Check that type can be XML serialized".Run(() => type.TrySerialize());
            var document = "Parsing type".Run(() => ObjectDocument.Parse(type));
            var outputPath = "Output path".Run(() => GetOutputPath(path, configuration));

            if (IncludeDocumentation)
            {
                var docPath = "Generating documentation path".Run(() => GetDocumentationPath(path, configuration));
                var documentation = "Setting up documentation source".Run(() => DocumentationSource.Create(document, docPath)
                    .SetInputFormat(DocumentationFileFormat)
                    .SetOutputFormat(DocumentationOutputFormat)
                    .SetEncoding(EncodeData)
                    .SetCharacterData(EncapsulateCharacterData)
                    .Build());

                var generator = "Creating XSD generator".Run(() => new XSDGenerator(document, documentation));
                var content = "Generating XSD".Run(() => generator.Run());
                File.WriteAllText(Path.Combine(outputPath, generator.FileName), content);
            }
            else
            {
                var generator = "Creating XSD generator".Run(() => new XSDGenerator(document));
                var content = "Generating XSD".Run(() => generator.Run());
                File.WriteAllText(Path.Combine(outputPath, generator.FileName), content);
            }
        }
    }
}
