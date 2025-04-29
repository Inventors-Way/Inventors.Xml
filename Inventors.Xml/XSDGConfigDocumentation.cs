using Inventors.Xml.Documentation;
using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml
{
    public class XSDGConfigDocumentation : IDocumentationSource
    {
        public string GetItem(string id) =>
            DocumentationContent.Assembly.ReadEmbeddedResourceString(id);
    }
}
