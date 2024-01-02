using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Test.TestObjects.Data
{
    public enum ProjectType
    {
        [XmlEnum("internal")]
        Internal,
        [XmlEnum("external")]
        External
    }
}
