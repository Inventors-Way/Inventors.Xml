using Inventors.Xml.Content;
using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Inventors.Xml.Analysis
{
    public class TypeAnalyser : Analyser
    {
        private readonly Dictionary<string, string> _typeMapping = new();

        public TypeAnalyser(ObjectDocument document, Reporter reporter) :
            base(document, reporter)
        {
            ChoiceAnalyser = new ChoiceAnalyser(this, document, reporter);
            ArrayAnalyser = new ArrayAnalyser(this, document, reporter);
            EnumAnalyser = new(document, reporter);

            _typeMapping.Add(typeof(double).ToString(), "double");
            _typeMapping.Add(typeof(float).ToString(), "float");
            _typeMapping.Add(typeof(string).ToString(), "string");
            _typeMapping.Add(typeof(int).ToString(), "integer");
            _typeMapping.Add(typeof(bool).ToString(), "boolean");
            _typeMapping.Add(typeof(long).ToString(), "long");
            _typeMapping.Add(typeof(short).ToString(), "short");
            _typeMapping.Add(typeof(byte).ToString(), "byte");
        }

        private ChoiceAnalyser ChoiceAnalyser { get; }

        private ArrayAnalyser ArrayAnalyser { get; }

        private EnumAnalyser EnumAnalyser { get; }

        public string Analyze(Type type)
        {
            if (!type.IsClass)
                throw new ArgumentException($"{type} is not a class");

            var generator = new OrderGenerator();
            var name = type.GetSchemaTypeName();

            if (Document.Exists(name))
                return name;

            TypeElement element = new(name, type.GetDocumentation());

            Document.Add(element);

            if (type.BaseType is Type baseType)
                Parse(baseType, element, generator);

            ParseProperties(type, element, generator);
            element.SortAttributes();

            return element.Name;
        }

        public void Parse(Type type, TypeElement element, OrderGenerator generator)
        {
            if (type.BaseType is Type baseType)
                Parse(baseType, element, generator);

            ParseProperties(type, element, generator);
        }
               
        public void ParseAttribute(PropertyInfo property, TypeElement element, OrderGenerator generator)
        {
            string typeKey = property.PropertyType.ToString();

            if (_typeMapping.ContainsKey(typeKey))
            {
                var name = property.GetAttributeName();
                var descriptor = new AttributeDescriptor(Name: name,
                                                         Type: _typeMapping[typeKey],
                                                         Required: property.IsPropertyRequired(),
                                                         Primitive: true,
                                                         Order: property.GetAttributeOrder(generator),
                                                         Documentation: property.GetDocumentation());
                element.Add(descriptor);
                return;
            }
            else if (property.PropertyType.IsEnum)
            {
                var name = property.GetAttributeName();
                var descriptor = new AttributeDescriptor(Name: name,
                                                         Type: EnumAnalyser.Analyse(property),
                                                         Required: property.IsPropertyRequired(),
                                                         Primitive: false,
                                                         Order: property.GetAttributeOrder(generator),
                                                         Documentation: property.GetDocumentation());
                element.Add(descriptor);
                return;
            }

            throw new InvalidOperationException($"Unsupported attribute type: {property.PropertyType}");
        }

        private void ParseProperties(Type type, TypeElement element, OrderGenerator generator)
        {
            foreach (var property in type.GetProperties())
            {
                var schemaType = property.GetSchemaType(type);

                switch (schemaType)
                {
                    case PropertyXSDType.Attribute:
                        ParseAttribute(property, element, generator);
                        break;

                    case PropertyXSDType.Element:
                        element.Add(new ElementDescriptor(Name: property.GetElementName(),
                                                            Type: Analyze(property.PropertyType),
                                                            Choice: false,
                                                            Required: property.IsPropertyRequired(),
                                                            Documentation: property.GetDocumentation()));
                        break;

                    case PropertyXSDType.Choice:
                        element.Add(new ElementDescriptor(Name: property.Name,
                                                            Type: ChoiceAnalyser.Analyse(element.Name, property),
                                                            Choice: true,
                                                            Required: property.IsPropertyRequired(),
                                                            Documentation: property.GetDocumentation()));
                        break;

                    case PropertyXSDType.Array:
                        element.Add(new ElementDescriptor(Name: property.GetArrayName(),
                                                            Type: ArrayAnalyser.Analyse(element.Name, property),
                                                            Choice: false,
                                                            Required: property.IsPropertyRequired(),
                                                            Documentation: property.GetDocumentation()));
                        break;

                    case PropertyXSDType.Private:
                    case PropertyXSDType.Inherited:
                    case PropertyXSDType.Ignored:
                        break;

                    case PropertyXSDType.Invalid:
                        throw new InvalidOperationException($"Parsing aborted as property [ {property.Name} ] in class [ {type.Name} ] has a public getter and setter but no information as how to serialize this property in XML. The XSDG tool requires all XML serialization to be explicit.");
                }
            }
        }        
    }
}
