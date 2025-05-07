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
            ArrayAnalyser = new ArrayAnalyser(document, reporter);
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

            var name = type.GetXSDTypeName();

            if (Document.Exists(name))
                return name;

            TypeElement element = new(name, type.GetDocumentation());

            Document.Add(element);

            Parse(type.BaseType, element);
            ParseProperties(type, element);

            return element.Name;
        }
        
        private void Parse(Type? type, TypeElement element)
        {
            if (type is null)
                return;

            Parse(type.BaseType, element);
            ParseProperties(type, element);
        }
        
        public void ParseAttribute(PropertyInfo property, TypeElement element)
        {
            string typeKey = property.PropertyType.ToString();

            if (_typeMapping.ContainsKey(typeKey))
            {
                var name = property.GetAttributeName();
                var descriptor = new AttributeDescriptor(Name: name,
                                                         Type: _typeMapping[typeKey],
                                                         Required: property.IsPropertyRequired(),
                                                         Primitive: true,
                                                         PropertyName: property.Name,
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
                                                         PropertyName: property.Name,
                                                         Documentation: property.GetDocumentation());
                element.Add(descriptor);
                return;
            }

            throw new InvalidOperationException($"Unsupported attribute type: {property.PropertyType}");
        }

        private void ParseProperties(Type type, TypeElement element)
        {
            foreach (var property in type.GetProperties())
            {
                switch (property.GetXSDType(type))
                {
                    case PropertyXSDType.Attribute:
                        ParseAttribute(property, element);
                        break;

                    case PropertyXSDType.Element:
                        element.Add(new ElementDescriptor(Name: property.GetElementName(),
                                                          Type: Analyze(property.PropertyType),
                                                          Required: property.IsPropertyRequired(),
                                                          PropertyName: property.Name,
                                                          Documentation: property.GetDocumentation()));
                        break;

                    case PropertyXSDType.Choice:
                        element.Add(new ElementDescriptor(Name: property.GetElementName(),
                                                          Type: ChoiceAnalyser.Analyse(element.Name, property),
                                                          Required: property.IsPropertyRequired(),
                                                          PropertyName: property.Name,
                                                          Documentation: property.GetDocumentation()));
                        break;

                    case PropertyXSDType.Array:
                        element.Add(new ElementDescriptor(Name: property.GetElementName(),
                                                          Type: ArrayAnalyser.Analyse(element.Name, property.PropertyType),
                                                          Required: property.IsPropertyRequired(),
                                                          PropertyName: property.Name,
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
