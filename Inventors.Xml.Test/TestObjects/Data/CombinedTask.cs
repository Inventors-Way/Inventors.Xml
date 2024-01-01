using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Test.TestObjects.Data
{
    public class CombinedTask : 
        Task
    {
        [XmlElement("task", typeof(SingleTask))]
        [XmlElement("repeated", typeof(RepeatedTask))]
        [XmlElement("combined", typeof(CombinedTask))]
        public List<Task> Tasks { get; } = new List<Task>();
    }
}
