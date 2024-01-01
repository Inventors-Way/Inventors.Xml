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
            base("", false)
        {

        }

        public override bool IsNested => false;

        public override void Accept(IElementVisitor visitor) => visitor.Visit(this);
    }

    public abstract class Element
    {
        private static readonly Element _empty = new NullElement();
        public Element(string name, bool isAbstract)
        {
            Name = name;
            BaseType = string.Empty;
            IsAbstract = isAbstract;
        }

        public string Name { get; }

        public string BaseType { get; internal set; }

        public bool IsAbstract { get; }

        public bool IsDerived => !string.IsNullOrEmpty(BaseType);

        public bool Null => string.IsNullOrEmpty(Name);

        public abstract bool IsNested { get; }

        public static Element Empty => _empty;

        public abstract void Accept(IElementVisitor visitor);
    }

}
