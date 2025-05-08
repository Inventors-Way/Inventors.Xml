using Inventors.Xml.Documentation;
using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Test.TestObjects
{
    public class UndocumentedPersonDocumentation : IDocumentationSource
    {
        public string GetItem(string id)
        {
            var assembly = typeof(UndocumentedPersonDocumentation).Assembly;

            try
            {
                return DocumentationContent.Assembly.ReadEmbeddedResourceString(id);
            }
            catch 
            {
                return $"Undocumented [ {id} ]";
            }
        }
    }
}
