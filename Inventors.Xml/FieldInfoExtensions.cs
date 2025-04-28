using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml
{
    public static class FieldInfoExtensions
    {
        public static string GetDocumentation(this FieldInfo property)
        {
            if (property.GetCustomAttribute<XmlDocumentationAttribute>() is XmlDocumentationAttribute documentation)
                return documentation.ID;

            return string.Empty;
        }
    }
}
