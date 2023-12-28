﻿using Inventors.Xml.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml
{
    public static class TypeExtensions
    {
        public static string RootElementName(this Type input)
        {
            var attributes = input.GetCustomAttributes(typeof(XmlRootAttribute), false);

            if (attributes.Length == 0)
                throw new ArgumentException($"No XmlRoot attribute specified for type {input}", nameof(input));

            if (attributes[0] is not XmlRootAttribute root)
                throw new ArgumentException($"No XmlRoot attribute specified for type {input}", nameof(input));

            if (root.ElementName is null)
                throw new ArgumentException($"No XmlRoot attribute specified for type {input}", nameof(input));

            return root.ElementName;
        }

        public static string GetXSDTypeName(this Type type)
        {
            if (type.FullName is not null)
                return type.FullName;

            return type.Name;
        }

        public static bool IsPropertyInherited(this Type type, string name)
        {
            if (type is null)
                return false;

            if (name is null)
                return false;

            Type? current = type.BaseType;

            while (current is not null)
            {
                PropertyInfo? property = current.GetProperty(name);

                if (property is not null)
                {
                    return true;
                }

                current = current.BaseType;
            }

            return false;
        }

        public static Element ParseClass(this Type type, ObjectDocument document)
        {
            if (!type.IsClass)
                throw new ArgumentException("Must a class to be parsed as a class", nameof(type));

            ClassElement element = new(name: type.GetXSDTypeName(),
                                       baseType: type.BaseType.ParseBaseType(document).Name,
                                       isAbstract: type.IsAbstract);

            document.Add(element);

            foreach (var property in type.GetProperties())
            {
                if (type.IsPropertyInherited(property.Name))
                    break;

                if (property.Ignore())
                    break;

                if (property.IsAttribute())
                {
                    var attribute = property.ParseAttribute(document);
                    element.Add(attribute);
                }
                else if (property.IsElement())
                {
                    if (property.IsChoiceElement())
                    {
                        var elementType = ParseChoiceElement($"{element.Name}.{property.Name}.ChoiceSet", property, document);
                        var elementDescriptor = new ElementDescriptor(Name: property.Name,
                                                                      Type: elementType,
                                                                      Required: property.IsPropertyRequired());
                        element.Add(elementDescriptor);
                    }
                    else
                    {
                        var elementType = ParseClass(property.PropertyType, document);
                        var elementDescriptor = new ElementDescriptor(Name: property.GetElementName(),
                                                                      Type: elementType,
                                                                      Required: property.IsPropertyRequired());
                        element.Add(elementDescriptor);
                    }
                }
                else if (property.IsArray())
                {
                    var elementType = ParseArrayElement($"{element.Name}.{property.Name}.Array", property, document);
                    var elementDescriptor = new ElementDescriptor(Name: property.GetArrayName(),
                                                                  Type: elementType,
                                                                  Required: property.IsPropertyRequired());
                    element.Add(elementDescriptor);
                }
                else if (property.IsPublic())
                {
                    throw new InvalidOperationException($"Property [ {property.Name} ] is public with no XML serialization information");
                }
            }

            return element;
        }

        private static Element ParseChoiceElement(string name, PropertyInfo property, ObjectDocument document)
        {
            List<Choice> choices = new();

            foreach (var choice in property.GetChoiceTypes())
            {
                choices.Add(new Choice(choice.Item1, choice.Item2.ParseClass(document)));
            }

            var element = new ChoiceElement(name, choices, property.IsEnumerable());
            document.Add(element);

            return element;
        }

        private static Element ParseArrayElement(string name, PropertyInfo property, ObjectDocument document)
        {
            List<ArrayItem> items = new();

            foreach (var item in property.GetArrayItems())
            {
                items.Add(new ArrayItem(item.Item1, item.Item2.ParseClass(document)));
            }

            var element = new ArrayElement(name, items);
            document.Add(element);

            return element;
        }

        private static Element ParseBaseType(this Type? baseType, ObjectDocument document)
        {
            if (baseType is null)
                return Element.Empty;

            if (baseType == typeof(Object))
                return Element.Empty;

            if (document.Exists(baseType.GetXSDTypeName()))
                return document[baseType.GetXSDTypeName()];

            return baseType.ParseClass(document);
        }

        public static IEnumerable<string> ParseEnumValues(this Type type)
        {
            if (!type.IsEnum)
                throw new ArgumentException("Is not an enum", nameof(type));

            foreach (var value in Enum.GetValues(type))
            {
                var name = $"{value}";
                var fieldInfo = type.GetField(name) ?? throw new InvalidOperationException($"No field into found for enum value {value}");

                if (Attribute.GetCustomAttribute(fieldInfo, typeof(XmlEnumAttribute)) is XmlEnumAttribute xmlEnum)
                    yield return xmlEnum.Name is null ? name : xmlEnum.Name;
                else
                    yield return name;
            }
        }
    }
}