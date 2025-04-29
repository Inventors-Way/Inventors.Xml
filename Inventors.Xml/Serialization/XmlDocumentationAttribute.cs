using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Enum | AttributeTargets.Field)]
    public class XmlDocumentationAttribute :
        Attribute
    {
        public XmlDocumentationAttribute(string id) 
        { 
            ID = id;
        }

        public XmlDocumentationAttribute() 
        { 
        }

        public string ID { get; } = string.Empty;

        public bool IsDocumented => !string.IsNullOrEmpty(ID);
    }

    public static class XmlDocumentationAttributeExtentions
    {
        public static bool IsDocumented(this Type self) =>
            self.GetCustomAttribute<XmlDocumentationAttribute>() is not null;

        public static string GetDocumentation(this Type self)
        {
            if (self.GetCustomAttribute<XmlDocumentationAttribute>() is not XmlDocumentationAttribute documentation)
                return string.Empty;

            return documentation.ID;
        }

        public static bool IsDocumented(this PropertyInfo self) =>
            self.GetCustomAttribute<XmlDocumentationAttribute>() is not null;

        public static string GetDocumentation(this PropertyInfo self)
        {
            if (self.GetCustomAttribute<XmlDocumentationAttribute>() is not XmlDocumentationAttribute documentation)
                return string.Empty;

            return documentation.ID;
        }

        public static bool IsDocumented(this FieldInfo self) =>
            self.GetCustomAttribute<XmlDocumentationAttribute>() is not null;

        public static string GetDocumentation(this FieldInfo self)
        {
            if (self.GetCustomAttribute<XmlDocumentationAttribute>() is not XmlDocumentationAttribute documentation)
                return string.Empty;

            return documentation.ID;
        }
    }
}
