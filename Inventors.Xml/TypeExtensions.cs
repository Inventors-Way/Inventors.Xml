﻿using Inventors.Xml.Content;
using Inventors.Xml.Generators.Xsd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static string GetXSDTypeName(this Type type) =>
            type.FullName is not null ? type.FullName : type.Name;

        public static bool IsPropertyInherited(this Type type, string name)
        {
            type.ThrowIfNull();
            name.ThrowIfNull();

            Type? current = type.BaseType;

            while (current is not null)
            {
                if (current.GetProperty(name) is not null)
                    return true;

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
                isAbstract: type.IsAbstract));

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
                        element.Add(ParseChoiceElement($"{element.Name}.{property.Name}.ChoiceSet", property, document, reporter));
                        break;
                    case PropertyXSDType.Array:
                        element.Add(ParseArrayElement($"{element.Name}.{property.Name}.Array", property, document, reporter));
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

        private static ElementDescriptor ParseChoiceElement(string name, PropertyInfo property, ObjectDocument document, Reporter reporter)
        {
            var element = document.Add(new ChoiceElement(name, property.IsEnumerable()));
            element.SetChoices(from choice in property.GetChoiceTypes()
                               select new Choice(choice.Item1, choice.Item2.ParseClass(document, reporter)));

            return new ElementDescriptor(Name: property.Name,
                                         Type: element,
                                         Required: property.IsPropertyRequired(),
                                         PropertyName: property.Name);
        }

        private static ElementDescriptor ParseArrayElement(string name, PropertyInfo property, ObjectDocument document, Reporter reporter)
        {
            var element = document.Add(new ArrayElement(name));
            element.SetItems(from item in property.GetArrayItems()
                             select new ArrayItem(item.Item1, item.Item2.ParseClass(document, reporter)));

            return new ElementDescriptor(Name: property.GetArrayName(),
                                         Type: element,
                                         Required: property.IsPropertyRequired(),
                                         PropertyName: property.Name); ;
        }

        private static Element ParseBaseType(this Type? baseType, ObjectDocument document, Reporter reporter)
        {
            if (baseType is null) return Element.Empty;
            if (baseType == typeof(Object)) return Element.Empty;
            if (document.Exists(baseType.GetXSDTypeName())) return document[baseType.GetXSDTypeName()];

            return baseType.ParseClass(document, reporter);
        }

        public static IEnumerable<EnumValue> ParseEnumValues(this Type type)
        {
            type.Throw().IfFalse(type.IsEnum);

            foreach (var value in Enum.GetValues(type))
            {
                var name = $"{value}";
                var fieldInfo = type.GetField(name) ?? throw new InvalidOperationException($"No field into found for enum value {value}");
                
                if (Attribute.GetCustomAttribute(fieldInfo, typeof(XmlEnumAttribute)) is XmlEnumAttribute xmlEnum)
                    yield return xmlEnum.Name is null ?
                                 new EnumValue(Name: name, XSDName: name) :
                                 new EnumValue(Name: name, XSDName: xmlEnum.Name);
                else
                    yield return new EnumValue(Name: name, XSDName: name);
            }
        }
    }
}
