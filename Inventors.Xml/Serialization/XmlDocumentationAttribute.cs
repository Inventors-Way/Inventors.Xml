using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Enum)]
    public class XmlDocumentationAttribute :
        Attribute
    {
        public XmlDocumentationAttribute(string documentation) 
        { 
            Documentation = documentation;
        }

        public XmlDocumentationAttribute() 
        { 
        }

        public string Documentation { get; } = string.Empty;
    }
}
