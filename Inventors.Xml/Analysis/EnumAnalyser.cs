using Inventors.Xml.Content;
using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Throw;

namespace Inventors.Xml.Analysis
{
    public class EnumAnalyser : Analyser
    {
        public EnumAnalyser(ObjectDocument document, Reporter reporter) : base(document, reporter)
        {
        }

        public string Analyse(PropertyInfo property)
        {
            if (property.PropertyType is not Type type)
                throw new ArgumentException("Invalid property passed to enum analyser");

            if (string.IsNullOrEmpty(type.FullName))
                throw new InvalidOperationException("Enum type does not have a full name");

            var typeName = type.FullName.Replace('+','.');

            if (Document.Exists(typeName))
                return typeName;

            Document.Add(new EnumElement(name: typeName,
                                         values: ParseEnumValues(type),
                                         documentation: type.GetDocumentation()));

            return typeName;
        }

        public static IEnumerable<EnumValue> ParseEnumValues(Type type)
        {
            if (!type.IsEnum)
                throw new ArgumentException($"{type} is not an enum");

            foreach (var value in Enum.GetValues(type))
            {
                var name = $"{value}";
                FieldInfo fieldInfo = type.GetField(name) ?? throw new InvalidOperationException($"No field into found for enum value {value}");

                if (Attribute.GetCustomAttribute(fieldInfo, typeof(XmlEnumAttribute)) is XmlEnumAttribute xmlEnum)
                    yield return xmlEnum.Name is null ?
                                 new EnumValue(Name: name, Type: name, Documentation: fieldInfo.GetDocumentation()) :
                                 new EnumValue(Name: name, Type: xmlEnum.Name, Documentation: fieldInfo.GetDocumentation());
                else
                    yield return new EnumValue(Name: name, Type: name, Documentation: fieldInfo.GetDocumentation());
            }
        }
    }
}
