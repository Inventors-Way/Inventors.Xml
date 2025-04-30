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
    [XmlDocumentation("@Person")]
    public class Person
    {
        [XmlAttribute("name")]
        [XmlRequired]
        [XmlDocumentation("The name of the person")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("spouse")]
        [XmlOptional]
        [XmlDocumentation("The spouse of the person")]
        public Spouse? Spouse { get; set; }

        [XmlElement("home")]
        [XmlOptional]
        [XmlDocumentation("@Person.Home")]
        public Home? Home { get; set; }

        [XmlArray("children")]
        [XmlArrayItem("child")]
        public List<Child> Children { get; } = new();

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"PERSON [ {Name} ]");

            if (Spouse is not null)
                builder.AppendLine($"   Spouse: {Spouse}");

            if (Home is not null)
                builder.AppendLine($"   Home: {Home}");

            if (Children.Count > 0)
            {
                builder.AppendLine("CHILDREN");

                foreach (var child in Children) 
                    builder.AppendLine(child.ToString());
            }

            return builder.ToString();
        }

    }
}
