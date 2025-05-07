using Inventors.Xml.Content;
using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Inventors.Xml.Analysis
{
    public class ArrayAnalyser : Analyser
    {
        public ArrayAnalyser(TypeAnalyser analyser, ObjectDocument document, Reporter reporter) : base(document, reporter)
        {
            Analyser = analyser;
        }

        public TypeAnalyser Analyser { get; }

        public string Analyse(string elementName, PropertyInfo property)
        {
            var name = $"{elementName}.{property.Name}.Array".Replace("+", ".");

            if (Document.Exists(name))
                return name;

            Document.Add(new ArrayElement(name, ParseArrayItems(property), property.GetDocumentation()));

            return name;
        }

        private IEnumerable<ArrayItem> ParseArrayItems(PropertyInfo property)
        {
            List<ArrayItem> retValue = new();

            foreach (var item in property.GetArrayItems())
            {
                var name = Analyser.Analyze(item.Item2);
                retValue.Add(new ArrayItem(item.Item1, name));
            }

            return retValue;
        }
    }
}
