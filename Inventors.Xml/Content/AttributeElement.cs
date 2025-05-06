using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Content
{
    public class AttributeElement : Element
    {
        public AttributeElement(string name, string primitiveType, string documentation) : base(name, documentation)
        {
            PrimitiveType = primitiveType;
        }

        public string PrimitiveType { get; }

        public override void Accept(IElementVisitor visitor) => visitor.Visit(this);
    }
}
