using System;
using System.Collections.Generic;
using System.Text;

namespace Inventors.Xml.Serialization
{
    [AttributeUsage(AttributeTargets.Property)]
    public class XmlOptionalAttribute :
        Attribute
    {
        public XmlOptionalAttribute(bool required)
        {
            Optional = required;
        }

        public XmlOptionalAttribute()
        {
            Optional = true;
        }

        public bool Optional { get; }
    }
}
