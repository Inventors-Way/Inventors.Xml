using System;

namespace Inventors.Xml.Serialization
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
