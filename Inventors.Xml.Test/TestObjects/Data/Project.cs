using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Test.TestObjects.Data
{
    public class Project
    {
        [XmlAttribute("project-type")]
        [XmlRequired(false)]
        public ProjectType ProjectType { get; set; } = ProjectType.Internal;

        [XmlAttribute("title")]
        [XmlRequired]
        public string Title { get; set; } = string.Empty;

        [XmlElement("task", typeof(SingleTask))]
        [XmlElement("repeated", typeof(RepeatedTask))]
        [XmlElement("combined", typeof(CombinedTask))]  
        public Task? Task { get; set; }
    }
}
