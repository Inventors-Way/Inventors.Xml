using Inventors.Xml.Content;
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
        public static PropertyXSDType GetSchemaType(this PropertyInfo property, Type type)
        {
            if (type.IsPropertyInherited(property.Name)) 
                return PropertyXSDType.Inherited;
            if (property.Ignore()) 
                return PropertyXSDType.Ignored;
            if (property.IsAttribute()) 
                return PropertyXSDType.Attribute;
            if (property.IsElement())
                return property.IsChoiceElement() ? PropertyXSDType.Choice : PropertyXSDType.Element;
            if (property.IsArray()) 
                return PropertyXSDType.Array;
            if (!property.IsPublic())
                return PropertyXSDType.Private;

            return PropertyXSDType.Invalid;
        }

        public static bool IsPublic(this PropertyInfo property) =>
            (property.GetSetMethod() is not null) && (property.GetGetMethod() is not null);

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

            Type? defaultType = null;

            if (property.PropertyType.GenericTypeArguments.Length > 0)
            {
                defaultType = property.PropertyType.GenericTypeArguments[0];
            }

            foreach (var item in property.GetCustomAttributes<XmlArrayItemAttribute>())
            {
                if (item.Type is not null)
                {
                    yield return (item.ElementName, item.Type);
                }
                else if (defaultType is not null)
                {
                    yield return (item.ElementName, defaultType);
                }
                else
                {
                    throw new InvalidOperationException($"No type is specified for ArrayItem {item.ElementName} and Generic type is specified");
                }
            }
        }

        public static bool IsEnumerable(this PropertyInfo property) =>
            property.PropertyType.GetInterfaces().Any((t) => t.Name.Contains("IEnumerable"));

        public static bool IsArray(this PropertyInfo property) =>
            property.GetCustomAttribute<XmlArrayAttribute>() is not null;

        public static IEnumerable<Type?> GetArrayTypes(this PropertyInfo property) =>
            from item in property.GetCustomAttributes<XmlArrayItemAttribute>()
            select item.Type;
        /*
        public static AttributeDescriptor ParseAttribute(this PropertyInfo property, ObjectDocument document)
        {
            string typeKey = property.PropertyType.ToString();

            if (_typeMapping.ContainsKey(typeKey))
            {
                return new AttributeDescriptor(Name: property.GetAttributeName(),
                                               Type: _typeMapping[typeKey],
                                               Required: property.IsPropertyRequired(),
                                               Primitive: true,
                                               PropertyName: property.Name,
                                               Documentation: property.GetDocumentation());
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
            type.FullName.ThrowIfNull();
            var typeName = TypeExtensions.SanitizeXSDName(type.FullName);

            if (!document.Exists(type.GetSchemaTypeName()))
            {
                document.Add(new EnumElement(name: typeName,
                                             values: type.ParseEnumValues(),
                                             documentation: property.GetDocumentation()));
            }

            return new AttributeDescriptor(
                Name: property.GetAttributeName(),
                Type: typeName,
                Required: property.IsPropertyRequired(),
                Primitive: false,
                PropertyName: property.Name,
                Documentation: property.GetDocumentation());
        }
        */
    }
}
