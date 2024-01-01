using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Test.TestObjects
{
    /// <summary>
    /// This class is intentionally written to fail XML reflection.
    /// It will fail because it does not have a parameterless contructor.
    /// </summary>
    public class InvalidReflectionClass
    {
        public InvalidReflectionClass(int count)
        {
            Count = count;
        }

        [XmlAttribute("count")]
        [XmlRequired(false)]
        public int Count { get; set; }
    }
}
