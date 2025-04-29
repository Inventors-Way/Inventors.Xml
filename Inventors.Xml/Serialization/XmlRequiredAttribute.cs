using System;
using System.Reflection;

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

    public static class XmlRequiredAttributeExtensions
    {
        public static bool IsPropertyRequired(this PropertyInfo self)
        {
            var required = self.GetCustomAttribute<XmlRequiredAttribute>();
            var optional = self.GetCustomAttribute<XmlOptionalAttribute>();

            if (required is not null)
            {
                if (optional is not null)
                    throw new InvalidOperationException($"The XmlOptional and XmlRequired attribute is mutually exclusive. They are both specified for the {self.Name} property");

                return required.Required;
            }

            if (optional is not null)
            {
                if (required is not null)
                    throw new InvalidOperationException($"The XmlOptional and XmlRequired attribute is mutually exclusive. They are both specified for the {self.Name} property");

                return !optional.Optional;
            }

            return false;
        }
    }
}
