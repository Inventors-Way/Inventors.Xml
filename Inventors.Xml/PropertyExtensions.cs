﻿using Inventors.Xml.Content;
using Inventors.Xml.Serialization;
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
    public static class PropertyExtensions
    {
        private static Dictionary<string, string> _typeMapping = new Dictionary<string, string>();

        static PropertyExtensions()
        {
            _typeMapping.Add(typeof(double).ToString(), "double");
            _typeMapping.Add(typeof(float).ToString(), "float");
            _typeMapping.Add(typeof(string).ToString(), "string");
            _typeMapping.Add(typeof(int).ToString(), "integer");
            _typeMapping.Add(typeof(bool).ToString(), "boolean");
            _typeMapping.Add(typeof(long).ToString(), "long");
            _typeMapping.Add(typeof(short).ToString(), "short");
            _typeMapping.Add(typeof(byte).ToString(), "byte");
        }

        public static PropertyXSDType GetXSDType(this PropertyInfo property, Type type)
        {
            if (type.IsPropertyInherited(property.Name)) 
                return PropertyXSDType.Inherited;
            if (property.Ignore()) 
                return PropertyXSDType.Ignored;
            if (property.IsAttribute()) 
                return PropertyXSDType.Attribute;
            if (property.IsElement())
                return property.IsChoiceElement() ? PropertyXSDType.Choice : PropertyXSDType.Class;
            if (property.IsArray()) 
                return PropertyXSDType.Array;
            if (!property.IsPublic()) 
                return PropertyXSDType.Private;

            return PropertyXSDType.Invalid;
        }

        public static bool IsPublic(this PropertyInfo property) =>
            (property.GetSetMethod() is not null) || (property.GetGetMethod() is not null);

        public static bool Ignore(this PropertyInfo property) =>
            property.GetCustomAttribute<XmlIgnoreAttribute>() is not null;

        public static bool IsAttribute(this PropertyInfo property) =>
            property.GetCustomAttribute<XmlAttributeAttribute>() is not null;

        public static string GetAttributeName(this PropertyInfo property) =>
            property.GetCustomAttribute<XmlAttributeAttribute>()
                .ThrowIfNull()
                .Value.AttributeName;

        public static bool IsElement(this PropertyInfo property) =>
            property.GetCustomAttributes<XmlElementAttribute>().Any();

        public static string GetElementName(this PropertyInfo property) =>
            property.GetCustomAttribute<XmlElementAttribute>()
                .ThrowIfNull()
                .Value.ElementName;

        public static bool IsChoiceElement(this PropertyInfo property) =>
            property.GetCustomAttributes<XmlElementAttribute>().Count() > 1 || property.IsEnumerable();

        public static Type? GetGenericDefault(PropertyInfo property)
        {
            if (!property.PropertyType.IsGenericType)
                return null;

            if (property.PropertyType.GenericTypeArguments.Length == 0)
                return null;

            return property.PropertyType.GenericTypeArguments[0];
        }

        public static IEnumerable<(string, Type)> GetChoiceTypes(this PropertyInfo property)
        {
            property.Throw("Is not a choice element").IfFalse(property.IsChoiceElement());

            Type? defaultType = GetGenericDefault(property);

            foreach (var element in property.GetCustomAttributes<XmlElementAttribute>())
            {
                if (element.Type is not null)
                {
                    yield return (element.ElementName, element.Type);
                }
                else if (defaultType is not null)
                {
                    yield return (element.ElementName, defaultType);
                }
            }
        }

        public static string GetArrayName(this PropertyInfo property)
        {
            if (property.GetCustomAttribute<XmlArrayAttribute>() is XmlArrayAttribute xmlArray)
                return xmlArray.ElementName;

            throw new ArgumentException("Is not a array element", nameof(property));
        }

        public static IEnumerable<(string, Type)> GetArrayItems(this PropertyInfo property)
        {
            if (!property.IsArray())
                throw new ArgumentException("Is not a array element", nameof(property));

            foreach (var item in property.GetCustomAttributes<XmlArrayItemAttribute>())
            {
                if (item.Type is not null)
                {
                    yield return (item.ElementName, item.Type);
                }
            }
        }

        public static bool IsEnumerable(this PropertyInfo property)
        {
            if (property is null)
                return false;

            if (property.PropertyType is null)
                return false;

            var type = property.PropertyType;

            return type.GetInterfaces().Any((t) => t.Name.Contains("IEnumerable"));
        }

        public static bool IsArray(this PropertyInfo property)
        {
            if (property is null)
                return false;

            return property.GetCustomAttribute<XmlArrayAttribute>() != null;
        }

        public static IEnumerable<Type?> GetArrayTypes(this PropertyInfo property)
        {
            if (!property.IsArray())
                throw new ArgumentException("Is not an array", nameof(property));

            foreach (var item in property.GetCustomAttributes<XmlArrayItemAttribute>())
            {
                yield return item.Type;
            }
        }

        public static AttributeDescriptor ParseAttribute(this PropertyInfo property, ObjectDocument document)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            if (document is null)
                throw new ArgumentNullException(nameof(document));

            if (property.PropertyType is null)
                throw new ArgumentException("The property does not have a type", nameof(property));

            string typeKey = property.PropertyType.ToString();

            if (_typeMapping.ContainsKey(typeKey))
            {
                return new AttributeDescriptor(Name: property.GetAttributeName(),
                                               Type: _typeMapping[typeKey],
                                               Required: property.IsPropertyRequired(),
                                               Primitive: true,
                                               PropertyName: property.Name);
            }
            else if (property.PropertyType.IsEnum)
            {
                return property.ParseEnum(document);
            }

            throw new InvalidOperationException($"Unsupported attribute type: {property.PropertyType}");
        }

        public static AttributeDescriptor ParseEnum(this PropertyInfo property, ObjectDocument document)
        {
            var type = property.PropertyType;

            if (type.FullName is null)
                throw new InvalidOperationException($"Enum type {type.Name} in attribute {property.Name} does not have a FullName");

            if (!document.Exists(type.GetXSDTypeName()))
            {
                var element = new EnumElement(name: type.FullName, 
                                              values: type.ParseEnumValues(),
                                              sourceValues: type.GetEnumStrings());
                document.Add(element);
            }

            return new AttributeDescriptor(Name: property.GetAttributeName(),
                                           Type: type.FullName,
                                           Required: property.IsPropertyRequired(),
                                           Primitive: false,
                                           PropertyName: property.Name);
        }

        public static bool IsPropertyRequired(this PropertyInfo property)
        {
            var required = property.GetCustomAttribute<XmlRequiredAttribute>();

            if (required is null)
                return false;

            return required.Required;
        }
    }

}
