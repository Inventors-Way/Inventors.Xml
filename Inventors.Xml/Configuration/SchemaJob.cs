using Inventors.Xml.Content;
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
            Console.WriteLine("Schema Job");
        }
    }
}
