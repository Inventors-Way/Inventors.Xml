using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Content
{
    public record AttributeDescriptor(string Name, string Type, bool Required, bool Primitive);
}
