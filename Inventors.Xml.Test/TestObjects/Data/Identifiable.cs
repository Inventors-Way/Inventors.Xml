using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Test.TestObjects.Data
{
    public abstract class Identifiable
    {
        [XmlAttribute("id")]
        [XmlRequired(true)]
        public string ID { get; set; } = "";
    }
}
