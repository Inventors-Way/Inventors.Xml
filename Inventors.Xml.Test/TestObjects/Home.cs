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
    [XmlDocumentation("Type of home")]
    public enum HomeType
    {
        [XmlEnum("appartment")]
        [XmlDocumentation("In a multi residence building")]
        Appartment,
        [XmlEnum("house")]
        [XmlDocumentation("A self-contrained unit")]
        House
    }

    [XmlDocumentation("@Home")]
    public class Home
    {
        [XmlAttribute("address")]
        [XmlRequired]
        [XmlDocumentation("@Home.Address")]
        public string Address { get; set; } = string.Empty;

        [XmlAttribute("home-type")]
        [XmlRequired]
        [XmlDocumentation("The type of home")]
        public HomeType HomeType { get; set; } = HomeType.Appartment;

        public override string ToString() => $"Home: {Address}";

    }
}
