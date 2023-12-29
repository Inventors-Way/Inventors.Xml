using Inventors.Xml.Configuration;
using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml
{
    [XmlRoot("xsdg")]
    public class XSDGConfig
    {
        [XmlAttribute("assembly")]
        [XmlRequired(true)]
        public string Assembly { get; set; } = string.Empty;

        [XmlAttribute("documentation-path")]
        [XmlRequired(false)]
        public string DocumentationPath { get; set; } = string.Empty;

        [XmlAttribute("output-path")]
        [XmlRequired(false)]
        public string OutputPath { get; set; } = string.Empty;

        [XmlAttribute("input-path")]
        [XmlRequired(false)]
        public string InputPath { get; set; } = string.Empty;

        [XmlElement("documentation", typeof(DocumentationJob))]
        [XmlElement("schema", typeof(SchemaJob))]
        public List<Job> Jobs { get; } = new List<Job>();
    }
}
