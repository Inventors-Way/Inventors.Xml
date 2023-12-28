using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Test.TestObjects.Data
{
    public enum EmployeeType
    {
        [XmlEnum("engineer")]
        Engineer,
        [XmlEnum("administrative")]
        Administrative
    }

    public class Employee :
        Identifiable
    {
        [XmlAttribute("name")]
        [XmlRequired(true)]
        public string Name { get; set; } = "";

        [XmlAttribute("address")]
        [XmlRequired(false)]
        public string Address { get; set; } = "";

        [XmlAttribute("salary")]
        [XmlRequired(true)]
        public double Salary { get; set; }

        [XmlAttribute("type")]
        [XmlRequired(false)]
        public EmployeeType Type { get; set; } = EmployeeType.Engineer;
    }
}
