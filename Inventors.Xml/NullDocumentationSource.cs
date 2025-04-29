using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml
{
    public class NullDocumentationSource : IDocumentationSource
    {
        public string GetItem(string id) => string.Empty;
    }
}
