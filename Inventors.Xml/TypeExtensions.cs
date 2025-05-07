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
        public static string GetSchema(this Type type) =>
            new XSDGenerator(ObjectDocument.Parse(type, NullReporter.Instance)).Run();

        public static string GetSchemaTypeName(this Type type) =>
            (type.FullName is not null ? type.FullName : type.Name).Replace("+", ".");

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
    }
}
