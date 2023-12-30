using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Content
{
    public record ArrayItem(string Name, Element Type);

    public class ArrayElement :
        Element
    {
        public ArrayElement(string name, IEnumerable<ArrayItem> items) :
            base(name: name, baseType: "", false)
        {
            Items = items.ToList();
        }

        public IList<ArrayItem> Items { get; }

        public override void Accept(IElementVisitor visitor) => visitor.Visit(this);

        public override bool IsNested => false;


        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"ARRAY [ name: {Name} ]");

            foreach (var item in Items)
            {
                builder.AppendLine($"- {item.Name} [ {item.Type.Name} ]");
            }

            return builder.ToString();
        }
    }
}
