using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Test.TestObjects
{
    [XmlDocumentation("@Spouse")]
    public class Spouse
    {
        [XmlAttribute("name")]
        [XmlRequired]
        [XmlDocumentation(@"
            The spouse's name 
            *Type: (<string>*")]
        public string Name { get; set; } = string.Empty;

        public override string ToString() => $"Spouse: {Name}";
    }
}
