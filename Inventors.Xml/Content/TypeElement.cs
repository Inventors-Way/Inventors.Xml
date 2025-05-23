﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Content
{
    public class TypeElement :
        Element
    {
        public TypeElement(string name, string documentation) :
            base(name: name, documentation: documentation)
        {
        }

        public IList<ElementDescriptor> Elements => _elements;

        public IList<AttributeDescriptor> Attributes => _attributes;

        public void SortAttributes()
        {
            _attributes.Sort((a,b) => a.Order.CompareTo(b.Order));
        }

        public void Add(AttributeDescriptor attribute) =>
            _attributes.Add(attribute);

        public void Add(ElementDescriptor element) =>
            _elements.Add(element);

        public override void Accept(IElementVisitor visitor) => visitor.Visit(this);

        public override string ToString()
        {
            StringBuilder builder = new();

            builder.AppendLine($"CLASS ELEMENT [ name: {Name} ]");

            if (Elements.Count > 0)
            {
                builder.AppendLine("ELEMENTS:");

                foreach (var element in Elements)
                {
                    builder.AppendLine($"- {element.Name} [ type: {element.Type}, required: {element.Required} ]");
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
