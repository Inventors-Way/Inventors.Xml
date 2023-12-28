using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Test.TestObjects
{
    [XmlRoot("company")]
    public class Company
    {
        [XmlArray("departments")]
        [XmlArrayItem("department", typeof(Department))]
        public List<Department> Departments { get; } = new List<Department>();
    }
}
