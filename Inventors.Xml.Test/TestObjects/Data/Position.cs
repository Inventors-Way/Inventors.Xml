using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Test.TestObjects.Data
{

    public class Position
    {
        [XmlAttribute("title")]
        [XmlRequired(true)]
        public string Title { get; set; } = string.Empty;

        [XmlAttribute("employee-id")]
        [XmlRequired(true)]
        public string EmployeeID { get; set; } = string.Empty;

        public override string ToString() => $"{Title} [ employee-id: {EmployeeID} ]";
    }
}
