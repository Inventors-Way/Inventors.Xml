using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Content
{
    public record AttributeDescriptor(string Name, string Type, bool Required, bool Primitive, int Order, string Documentation);
    public record ElementDescriptor(string Name, string Type, bool Choice, bool Required, string Documentation);
    public record EnumValue(string Name, string Documentation);
    public record ArrayItem(string Name, string Type);
    public record Choice(string Name, string Type);
}
