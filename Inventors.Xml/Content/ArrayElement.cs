using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Content
{
    public class ArrayElement :
        Element
    {
        public ArrayElement(string name, IEnumerable<ArrayItem> items, string documentation) :
            base(name: name, documentation)
        {
            _items.AddRange(items);
        }

        public IList<ArrayItem> Items => _items;

        public override void Accept(IElementVisitor visitor) => visitor.Visit(this);

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"ARRAY [ name: {Name} ]");

            foreach (var item in Items)
            {
                builder.AppendLine($"- {item.Name} [ {item.Type} ]");
            }

            return builder.ToString();
        }

        private readonly List<ArrayItem> _items = new(); 
    }
}
