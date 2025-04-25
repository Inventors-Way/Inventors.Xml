using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Enum)]
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
}
