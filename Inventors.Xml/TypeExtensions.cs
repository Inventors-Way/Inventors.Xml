using Inventors.Xml.Content;
using Inventors.Xml.Generators.Xsd;
using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Throw;

namespace Inventors.Xml
{
    public static class TypeExtensions
    {
        public static string GetSchema(this Type type)
        {
            return new XSDGenerator(ObjectDocument.Parse(type, NullReporter.Instance)).Run();
        }

        public static string RootElementName(this Type input) =>
            (input.GetCustomAttributes(typeof(XmlRootAttribute), false)
                .ThrowIfNull()
                .IfEmpty()
                .Value[0] as XmlRootAttribute)
                .ThrowIfNull()
                .IfTrue(root => root.ElementName is null)
                .Value.ElementName;

        public static string SanitizeXSDName(string name) =>
            name.Replace("+", ".");

        public static string GetXSDTypeName(this Type type)
        {
            var name = type.FullName is not null ? type.FullName : type.Name;
            return SanitizeXSDName(name);            
        }

        public static bool IsPropertyInherited(this Type type, string name)
        {
            type.ThrowIfNull();
            name.ThrowIfNull();

            Type? current = type.BaseType;

            while (current is not null)
            {
                if (current.GetProperty(name) is PropertyInfo property)
                {
                    bool isAbstract = (property.GetGetMethod()?.IsAbstract ?? false) || 
                                      (property.GetSetMethod()?.IsAbstract ?? false);

                    return !isAbstract;
                }

                current = current.BaseType;
            }

            return false;
        }

        public static Element ParseClass(this Type type, ObjectDocument document, Reporter reporter)
        {
            type.Throw().IfFalse(type => type.IsClass);

            reporter.Report($"Parsing of class [ name: {type.Name} ]:");
            reporter.Report($"   XSD Name: {type.GetXSDTypeName()}");

            var element = document.Add(new ClassElement(
                name: type.GetXSDTypeName(),
                isAbstract: type.IsAbstract,
                documentation: type.GetDocumentation()));

            element.BaseType = type.BaseType.ParseBaseType(document, reporter).Name;

            foreach (var property in type.GetProperties())
            {
                switch (property.GetXSDType(type))
                {
                    case PropertyXSDType.Attribute:
                        element.Add(property.ParseAttribute(document));
                        break;
                    case PropertyXSDType.Class:
                        element.Add(new ElementDescriptor(Name: property.GetElementName(),
                                                          Type: ParseClass(property.PropertyType, document, reporter),
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

            return element;
        }

        private static List<Choice> ParseChoices(this PropertyInfo property, ObjectDocument document, Reporter reporter)
        {
            List<Choice> retValue = new List<Choice>();

            foreach (var choice in property.GetChoiceTypes())
            {
                var instance = choice.Item2.ParseClass(document, reporter);
                retValue.Add(new Choice(choice.Item1, instance));
            }

            return retValue;
        }

        private static Element ParseChoiceElement(string name, PropertyInfo property, ObjectDocument document, Reporter reporter)
        {
            if (document.Exists(name))
                return document[name];

            var element = document.Add(new ChoiceElement(name, property.IsEnumerable(), property.GetDocumentation()));
            var choices = property.ParseChoices(document, reporter);
            element.SetChoices(choices);

            return element;
        }

        private static List<ArrayItem> ParseArrayItems(this PropertyInfo property, ObjectDocument document, Reporter reporter)
        {
            List<ArrayItem> retValue = new List<ArrayItem>();

            try
            {

                foreach (var item in property.GetArrayItems())
                {
                    var instance = item.Item2.ParseClass(document, reporter);
                    retValue.Add(new ArrayItem(item.Item1, instance));
                }
            }
            catch (InvalidOperationException ioe)
            {
                throw new InvalidOperationException($"Error in reflecting property {property.Name}", ioe);
            }

            return retValue;
        }

        private static Element ParseArrayElement(string name, PropertyInfo property, ObjectDocument document, Reporter reporter)
        {
            if (document.Exists(name))
                return document[name];

            var element = document.Add(new ArrayElement(name, property.GetDocumentation()));
            var items = property.ParseArrayItems(document, reporter);
            element.SetItems(items);

            return element; 
        }

        private static bool IsSystemType(string name)
        {
            return name.StartsWith("System");
        }

        private static Element ParseBaseType(this Type? baseType, ObjectDocument document, Reporter reporter)
        {
            if (baseType is null) return Element.Empty;
            if (baseType.FullName is null) return Element.Empty; 
            if (IsSystemType(baseType.FullName)) return Element.Empty;
            if (document.Exists(baseType.GetXSDTypeName())) return document[baseType.GetXSDTypeName()];

            return baseType.ParseClass(document, reporter);
        }

        public static IEnumerable<EnumValue> ParseEnumValues(this Type type)
        {
            type.Throw().IfFalse(type.IsEnum);

            foreach (var value in Enum.GetValues(type))
            {
                var name = $"{value}";
                FieldInfo fieldInfo = type.GetField(name) ?? throw new InvalidOperationException($"No field into found for enum value {value}");
                
                if (Attribute.GetCustomAttribute(fieldInfo, typeof(XmlEnumAttribute)) is XmlEnumAttribute xmlEnum)
                    yield return xmlEnum.Name is null ?
                                 new EnumValue(Name: name, XSDName: name, Documentation: fieldInfo.GetDocumentation()) :
                                 new EnumValue(Name: name, XSDName: xmlEnum.Name, Documentation: fieldInfo.GetDocumentation());
                else
                    yield return new EnumValue(Name: name, XSDName: name, Documentation: fieldInfo.GetDocumentation());
            }
        }

        public static string GetDocumentation(this Type property)
        {
            if (property.GetCustomAttribute<XmlDocumentationAttribute>() is XmlDocumentationAttribute documentation)
                return documentation.ID;

            return string.Empty;
        }
    }
}
