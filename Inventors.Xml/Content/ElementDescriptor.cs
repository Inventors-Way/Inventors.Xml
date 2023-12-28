using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Content
{
    public record ElementDescriptor(string Name, Element Type, bool Required, string PropertyName);
}
