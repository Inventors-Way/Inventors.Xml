using Inventors.Xml.Analysis;
using Inventors.Xml.Content;
using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml
{
    public class ObjectDocument
    {
        public static ObjectDocument Parse(Type type, Reporter reporter) => new(type, reporter);

        private ObjectDocument(Type type, Reporter reporter)
        {
            if (type.Namespace is null)
                throw new ArgumentException("Has no Namespace", nameof(type));

            TypeAnalyser analyser = new(this, reporter);

            Namespace = type.Namespace;
            var rootName = analyser.Analyze(type);
            Root = new ElementDescriptor(
                Name: type.RootElementName(), 
                Type: rootName, 
                Required: false, 
                PropertyName: "",
                Documentation: type.GetDocumentation());
        }

        public Element this[string id]
        {
            get
            {
                if (!Exists(id))
                    return Element.Empty;

                return _types[id];
            }
        }

        public void Run(IElementVisitor visitor, bool runNestedTypes = false)
        {
            foreach (var item in _types)
            {
                item.Value.Accept(visitor);
            }
        }

        public ElementDescriptor Root { get; }

        public string Namespace { get; }

        public bool Exists(string name)
        {
            return _types.ContainsKey(name);
        }

        public T Add<T>(T element)
            where T : Element
        {
            if (_types.ContainsKey(element.Name))
                return element;

            _types.Add(element.Name, element);
            return element;
        }

        public override string ToString()
        {
            StringBuilder builder = new();

            builder.AppendLine($"ROOT ELEMENT [name: {Root.Name}, type: {Root.Name}, namespace: {Namespace}]");
            builder.AppendLine();

            foreach (var entry in _types)
            {
                builder.AppendLine(entry.Value.ToString());
            }


            return builder.ToString();
        }

        private readonly Dictionary<string, Element> _types = new();
    }
}

