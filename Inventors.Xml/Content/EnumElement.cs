using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Content
{
    public class EnumElement :
        Element
    {
        public EnumElement(string name, IEnumerable<string> values, IEnumerable<string> sourceValues) :
            base(name: name, baseType: "", false)
        {
            Values = values.ToList();
            SourceValues = sourceValues.ToList();
        }

        public override bool IsNested => false;

        public override void Accept(IElementVisitor visitor) => visitor.Visit(this);

        public override string ToString()
        {
            StringBuilder builder = new();

            builder.AppendLine($"ENUM ELEMENT [ name: {Name} ]");

            foreach (var value in Values)
            {
                builder.AppendLine($"- {value}");
            }

            return builder.ToString();
        }

        public IList<string> Values { get; }

        public IList<string> SourceValues { get; }
    }
}
