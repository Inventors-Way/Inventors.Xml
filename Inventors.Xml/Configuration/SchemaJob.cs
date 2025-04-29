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

        private XSDGenerator CreateGenerator(ObjectDocument document)
        {
            if (!IncludeDocumentation)
                return "Creating XSD generator".Run(() => new XSDGenerator(document));

            var documentation = "Setting up documentation source".Run(() => DocumentationProvider.Create(document, new XSDGConfigDocumentation())
                .SetInputFormat(DocumentationFileFormat)
                .SetOutputFormat(DocumentationOutputFormat)
                .SetEncoding(EncodeData)
                .SetCharacterData(EncapsulateCharacterData)
            .Build());

            return "Creating XSD generator".Run(() => new XSDGenerator(document, documentation));
        }

        public override void Run(string path, IJobConfiguration configuration, bool verbose = false)
        {
            var reporter = new ConsoleReporter(verbose);
            var type = $"Loading type: {Type}".Run(() => LoadType(configuration));
            "Check that type can be XML serialized".Run(() => type.TrySerialize());
            var document = "Parsing type".Run(() => ObjectDocument.Parse(type, reporter));
            var outputPath = "Output path".Run(() => GetOutputPath(path, configuration));
            var generator = CreateGenerator(document);
            var content = "Generating XSD".Run(() => generator.Run());
            File.WriteAllText(Path.Combine(outputPath, generator.FileName), content);
        }
    }
}
