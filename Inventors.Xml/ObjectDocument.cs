using Inventors.Xml.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml
{
    public class ObjectDocument
    {
        public static ObjectDocument Create(Type type) => new(type);

        private ObjectDocument(Type type)
        {
            if (type.Namespace is null)
                throw new ArgumentException("Has no Namespace", nameof(type));

            Namespace = type.Namespace;
            Root = new ElementDescriptor(Name: type.RootElementName(), type.ParseClass(this), false, "");
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
                if (!item.Value.IsNested || runNestedTypes)
                {
                    item.Value.Accept(visitor);
                }
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

            builder.AppendLine($"ROOT ELEMENT [name: {Root.Name}, type: {Root.Type.Name}, namespace: {Namespace}]");
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

