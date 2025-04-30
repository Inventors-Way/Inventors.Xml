using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Test.TestObjects
{
    [XmlRoot("person")]
    [XmlDocumentation("Person")]
    public class Person
    {
        [XmlAttribute("name")]
        [XmlRequired]
        [XmlDocumentation("Person.Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("spouse")]
        [XmlOptional]
        [XmlDocumentation("Person.Spouse")]
        public Spouse? Spouse { get; set; }

        [XmlElement("home")]
        [XmlOptional]
        [XmlDocumentation("Person.Home")]
        public Home? Home { get; set; }

        [XmlArray("children")]
        [XmlArrayItem("child")]
        public List<Child> Children { get; } = new();

    }
}
