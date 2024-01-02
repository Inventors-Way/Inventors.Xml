using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Content
{
    public class ClassElement :
        Element
    {
        public ClassElement(string name, bool isAbstract) :
            base(name: name, isAbstract: isAbstract)
        {
        }

        public override bool IsNested => false;


        public IList<ElementDescriptor> Elements => _elements;

        public IList<AttributeDescriptor> Attributes => _attributes;

        public void Add(AttributeDescriptor attribute) =>
            _attributes.Add(attribute);

        public void Add(ElementDescriptor element) =>
            _elements.Add(element);

        public override void Accept(IElementVisitor visitor) => visitor.Visit(this);

        public override string ToString()
        {
            StringBuilder builder = new();

            builder.AppendLine($"CLASS ELEMENT [ name: {Name}, abstract: {IsAbstract} ]");

            if (!string.IsNullOrEmpty(BaseType))
            {
                builder.AppendLine($" - BASETYPE: {BaseType}");
            }

            if (Elements.Count > 0)
            {
                builder.AppendLine("ELEMENTS:");

                foreach (var element in Elements)
                {
                    builder.AppendLine($"- {element.Name} [ type: {element.Type.Name}, required: {element.Required} ]");
                }
            }

            if (Attributes.Count > 0)
            {
                builder.AppendLine("ATTRIBUTES");

                foreach (var attribute in Attributes)
                {
                    builder.AppendLine($"- {attribute.Name} [ type: {attribute.Type}, required: {attribute.Required} ]");
                }
            }

            return builder.ToString();
        }

        private readonly List<ElementDescriptor> _elements = new();
        private readonly List<AttributeDescriptor> _attributes = new();
    }
}
