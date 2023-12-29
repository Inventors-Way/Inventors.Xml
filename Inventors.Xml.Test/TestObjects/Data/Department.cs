using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Test.TestObjects.Data
{
    public class Department :
        Identifiable
    {
        [XmlAttribute("name")]
        [XmlRequired(true)]
        public string Name { get; set; } = string.Empty;

        [XmlAttribute("manager")]
        public string Manager { get; set; } = string.Empty;

        [XmlElement("position")]
        public List<Position> Positions { get; } = new();

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"- {Name} [ Manager: {Manager} ]");

            foreach (var position in Positions)
            {
                builder.AppendLine($"   - {position}");
            }

            return builder.ToString();
        }
    }
}
