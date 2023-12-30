using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Content
{
    public record AttributeDescriptor(string Name, string Type, bool Required, bool Primitive, string PropertyName);
    public record ElementDescriptor(string Name, Element Type, bool Required, string PropertyName);
    public record EnumValue(string Name, string XSDName);
    public record ArrayItem(string Name, Element Type);
    public record Choice(string Name, Element Type);


}
