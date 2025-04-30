using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Test.TestObjects
{
    public class PersonDocumentation : IDocumentationSource
    {
        public string GetItem(string id) => $"PersonDocumentation: {id}";
    }
}
