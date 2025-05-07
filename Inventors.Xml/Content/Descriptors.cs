using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Content
{
    public record AttributeDescriptor(string Name, string Type, bool Required, bool Primitive, string PropertyName, string Documentation);
    public record ElementDescriptor(string Name, string Type, bool Required, string PropertyName, string Documentation);
    public record EnumValue(string Name, string Type, string Documentation);
    public record ArrayItem(string Name, string Type);
    public record Choice(string Name, string Type);
}
