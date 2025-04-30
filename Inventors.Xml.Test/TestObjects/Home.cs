using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Inventors.Xml.Test.TestObjects
{
    [XmlDocumentation("Home")]
    public class Home
    {
        [XmlAttribute("address")]
        [XmlRequired]
        [XmlDocumentation("Home.Address")]
        public string Address { get; set; } = string.Empty;

        public override string ToString() => $"Home: {Address}";

    }
}
