using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Content
{
    public class NullElement :
        Element
    {
        public NullElement() :
            base("", "")
        {

        }

        public override void Accept(IElementVisitor visitor) => visitor.Visit(this);
    }

    public abstract class Element
    {
        private static readonly Element _empty = new NullElement();

        public Element(string name, string documentation)
        {
            Name = name;
            Documentation = documentation;
        }

        public string Name { get; }

        public bool Null => string.IsNullOrEmpty(Name);

        public static Element Empty => _empty;

        public string Documentation { get; } = string.Empty;

        public bool IsDocumented => !string.IsNullOrEmpty(Documentation);

        public abstract void Accept(IElementVisitor visitor);
    }

}
