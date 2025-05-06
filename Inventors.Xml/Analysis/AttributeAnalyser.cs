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
    public class AttributeAnalyser : Analyser
    {
        private readonly Dictionary<string, string> _typeMapping = new();

        public AttributeAnalyser(ObjectDocument document, Reporter reporter) : base(document, reporter)
        {
            _typeMapping.Add(typeof(double).ToString(), "double");
            _typeMapping.Add(typeof(float).ToString(), "float");
            _typeMapping.Add(typeof(string).ToString(), "string");
            _typeMapping.Add(typeof(int).ToString(), "integer");
            _typeMapping.Add(typeof(bool).ToString(), "boolean");
            _typeMapping.Add(typeof(long).ToString(), "long");
            _typeMapping.Add(typeof(short).ToString(), "short");
            _typeMapping.Add(typeof(byte).ToString(), "byte");

            EnumAnalyser = new(document, reporter);
        }

        private EnumAnalyser EnumAnalyser { get; }

        public string Analyze(PropertyInfo property)
        {
            string typeKey = property.PropertyType.ToString();

            if (_typeMapping.ContainsKey(typeKey))
            {
                var name = property.GetAttributeName();
                var element = new AttributeElement(name, _typeMapping[typeKey], property.GetDocumentation());

                return name;
            }
            else if (property.PropertyType.IsEnum)
            {
                return property.ParseEnum(document);
            }

            throw new InvalidOperationException($"Unsupported attribute type: {property.PropertyType}");

        }
    }
}
