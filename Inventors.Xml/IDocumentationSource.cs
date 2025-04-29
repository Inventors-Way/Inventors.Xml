using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml
{
    public interface IDocumentationSource
    {
        string GetItem(string id);
    }
}
