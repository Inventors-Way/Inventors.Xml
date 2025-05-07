using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Content
{
    public class ChoiceElement :
        Element
    {
        public ChoiceElement(string name, bool multiple, string documentation) :
            base(name: name, documentation)
        {
            Multiple = multiple;
        }

        public void SetChoices(IList<Choice> choices)
        {
            _choices.AddRange(choices);
        }

        public bool Multiple { get; }

        public IList<Choice> Choices => _choices;

        public override void Accept(IElementVisitor visitor) => visitor.Visit(this);

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

        private readonly List<Choice> _choices = new();
    }
}
