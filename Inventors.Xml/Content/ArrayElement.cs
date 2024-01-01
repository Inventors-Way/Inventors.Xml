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
        public ArrayElement(string name) :
            base(name: name, false)
        {
        }

        internal void SetItems(IEnumerable<ArrayItem> items)
        {
            _items.AddRange(items); 
        }

        public IList<ArrayItem> Items => _items;

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

        private readonly List<ArrayItem> _items = new(); 
    }
}
