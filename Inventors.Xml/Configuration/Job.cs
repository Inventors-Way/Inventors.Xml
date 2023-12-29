using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Configuration
{
    public abstract class Job
    {
        [XmlAttribute("type")]
        [XmlRequired(true)]
        public string Type { get; set; } = string.Empty;

    }
}
