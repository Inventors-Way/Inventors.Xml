using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml
{
    [AttributeUsage(AttributeTargets.Property)]
    public class XmlRequiredAttribute :
        Attribute
    {
        public XmlRequiredAttribute(bool required)
        {
            Required = required;
        }

        public XmlRequiredAttribute()
        {
            Required = true;
        }

        public bool Required { get; }
    }
}
