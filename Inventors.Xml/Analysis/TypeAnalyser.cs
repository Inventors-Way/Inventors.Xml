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
    public class TypeAnalyser : Analyser
    {
        public TypeAnalyser(ObjectDocument document, Reporter reporter) :
            base(document, reporter)
        {
            AttributeAnalyser = new AttributeAnalyser(document, reporter);
            ChoiceAnalyser = new ChoiceAnalyser(document, reporter);
            ArrayAnalyser = new ArrayAnalyser(document, reporter);
        }

        private AttributeAnalyser AttributeAnalyser { get; }

        private ChoiceAnalyser ChoiceAnalyser { get; }

        private ArrayAnalyser ArrayAnalyser { get; }


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

        private void ParseProperties(Type type, TypeElement element)
        {
            foreach (var property in type.GetProperties())
            {
                switch (property.GetXSDType(type))
                {
                    case PropertyXSDType.Attribute:
                        element.Add(property.ParseAttribute(Document));
                        break;
                    case PropertyXSDType.Element:
                        element.Add(new ElementDescriptor(Name: property.GetElementName(),
                                                          Type: Analyze(property.PropertyType),
                                                          Required: property.IsPropertyRequired(),
                                                          PropertyName: property.Name));
                        break;
                    case PropertyXSDType.Choice:
                        {
                            var name = SanitizeXSDName($"{element.Name}.{property.Name}.ChoiceSet");
                            var choiceElement = ParseChoiceElement(name, property, document, reporter);

                            element.Add(new ElementDescriptor(Name: property.Name,
                                         Type: choiceElement,
                                         Required: property.IsPropertyRequired(),
                            PropertyName: property.Name));

                            document.Add(choiceElement);
                        }
                        break;
                    case PropertyXSDType.Array:
                        {
                            try
                            {
                                var name = SanitizeXSDName($"{element.Name}.{property.Name}.Array");
                                var arrayElement = ParseArrayElement(name, property, document, reporter);
                                element.Add(new ElementDescriptor(Name: property.GetArrayName(),
                                             Type: arrayElement,
                                             Required: property.IsPropertyRequired(),
                                             PropertyName: property.Name));
                            }
                            catch (InvalidOperationException ioe)
                            {
                                throw new InvalidOperationException($"Error in reflecting class {element.Name}", ioe);
                            }
                        }
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
