using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Test.TestObjects
{
    public class Organization :
        Identifiable
    {
        [XmlAttribute("name")]
        [XmlRequired(true)]
        public string Name { get; set; } = "";


    }
}
