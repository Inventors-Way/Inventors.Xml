using Inventors.Xml.Content;
using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Analysis
{
    public class ChoiceAnalyser : Analyser
    {
        public ChoiceAnalyser(TypeAnalyser analyser, ObjectDocument document, Reporter reporter) : base(document, reporter)
        {
            Analyser = analyser;
        }

        public TypeAnalyser Analyser { get; }

        public string Analyse(string elementName, PropertyInfo property)
        {
            var name = $"{elementName}.{property.Name}.ChoiceSet".Replace("+", ".");

            if (Document.Exists(name))
                return name;

            var element = ParseChoiceElement(name, property);
            Document.Add(element);

            return name;
        }

        private Element ParseChoiceElement(string name, PropertyInfo property)
        {
            var element = Document.Add(new ChoiceElement(name, property.IsEnumerable(), property.GetDocumentation()));
            var choices = ParseChoices(property);
            element.SetChoices(choices);

            return element;
        }

        private List<Choice> ParseChoices(PropertyInfo property)
        {
            List<Choice> retValue = new();

            foreach (var choice in property.GetChoiceTypes())
            {
                var name = Analyser.Analyze(choice.Item2);
                retValue.Add(new Choice(choice.Item1, name));
            }

            return retValue;
        }
    }
}
