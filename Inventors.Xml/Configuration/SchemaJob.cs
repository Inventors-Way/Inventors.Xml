using Inventors.Xml.Content;
using Inventors.Xml.Generators.Xsd;
using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Inventors.Xml.Configuration
{
    public class SchemaJob :
        Job
    {
        [XmlAttribute("include-documentation")]
        [XmlRequired(false)]
        [XmlDocumentation("Configuration.SchemaJob.IncludeDocumentation.md")]
        public bool IncludeDocumentation { get; set; } = true;

        [XmlAttribute("documentation-file-format")]
        [XmlRequired(false)]
        [XmlDocumentation("Configuration.SchemaJob.DocumentationFileFormat.md")]
        public DocumentationFormat DocumentationFileFormat { get; set; } = DocumentationFormat.MarkDown;

        [XmlAttribute("documentation-output-format")]
        [XmlRequired(false)]
        [XmlDocumentation("Configuration.SchemaJob.DocumentationOutputFormat.md")]
        public DocumentationFormat DocumentationOutputFormat { get; set; } = DocumentationFormat.Html;

        [XmlAttribute("encode-data")]
        [XmlRequired(false)]
        [XmlDocumentation("Configuration.SchemaJob.EncodeData.md")]
        public bool EncodeData { get; set; } = true;

        [XmlAttribute("encapsulate-character-data")]
        [XmlRequired(false)]
        [XmlDocumentation("Configuration.SchemaJob.EncapsulateCharacterData.md")]
        public bool EncapsulateCharacterData { get; set; } = false;

        private XSDGenerator CreateGenerator(ObjectDocument document, IDocumentationSource? docSource)
        {
            if (!IncludeDocumentation)
                return "Creating XSD generator".Run(() => new XSDGenerator(document));

            var documentation = "Setting up documentation source".Run(() => DocumentationProvider.Create(document, docSource)
                .SetInputFormat(DocumentationFileFormat)
                .SetOutputFormat(DocumentationOutputFormat)
                .SetEncoding(EncodeData)
                .SetCharacterData(EncapsulateCharacterData)
            .Build());

            return "Creating XSD generator".Run(() => new XSDGenerator(document, documentation));
        }

        public override void Run(string path, IJobConfiguration configuration, IDocumentationSource? docSource = null, bool verbose = false)
        {
            var reporter = new ConsoleReporter(verbose);
            var type = $"Loading type: {Type}".Run(() => LoadType(configuration));
            "Check that type can be XML serialized".Run(() => type.TrySerialize());
            var document = "Parsing type".Run(() => ObjectDocument.Parse(type, reporter));
            var outputPath = "Output path".Run(() => GetOutputPath(path, configuration));
            var generator = CreateGenerator(document, docSource);
            var content = "Generating XSD".Run(() => generator.Run());
            File.WriteAllText(Path.Combine(outputPath, generator.FileName), content);
        }
    }
}
