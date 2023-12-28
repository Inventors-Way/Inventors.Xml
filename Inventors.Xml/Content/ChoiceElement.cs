using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Content
{
    public record Choice(string Name, Element Type);

    public class ChoiceElement :
        Element
    {
        public ChoiceElement(string name, IList<Choice> choices, bool multiple) :
            base(name: name, baseType: "", false)
        {
            Choices = choices;
            Multiple = multiple;
        }

        public bool Multiple { get; }

        public IList<Choice> Choices { get; }

        public override void Accept(IElementVisitor visitor) => visitor.Visit(this);

        public override bool IsNested => true;

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"CHOICE [ name: {Name}, enumrable: {Multiple}]");

            foreach (var choice in Choices)
            {
                builder.AppendLine($"- {choice.Name} [ {choice.Type.Name} ]");
            }

            return builder.ToString();
        }
    }
}
