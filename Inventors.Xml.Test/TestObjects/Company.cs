using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Inventors.Xml.Test.TestObjects.Data;

namespace Inventors.Xml.Test.TestObjects
{
    [XmlRoot("company")]
    public class Company
    {
        [XmlAttribute("name")]
        [XmlRequired(true)]
        public string Name { get; set; } = string.Empty;

        [XmlArray("departments")]
        [XmlArrayItem("department", typeof(Department))]
        public List<Department> Departments { get; } = new List<Department>();

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"COMPANY [ {Name} ]");
            builder.AppendLine();
            builder.AppendLine("DEPARTMENTS:");

            foreach (var department in Departments )
            {
                builder.AppendLine(department.ToString());
            }

            return builder.ToString();
        }
    }
}
