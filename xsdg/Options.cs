using Inventors.Xml.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace xsdg
{
    public abstract class Options
    {
        protected static Type LoadType(string assemblyName, string typeName)
        {
            if (!File.Exists(assemblyName))
            {
                Console.WriteLine($"Did not find assembly: {assemblyName}");
                throw new ArgumentException("Assembly not found");
            }

            var assembly = Assembly.LoadFrom(assemblyName);
            var type = assembly.GetType(typeName);

            if (type is null)
            {
                throw new InvalidOperationException($"Failed to load type [ {typeName} ]");
            }

            return type;
        }

        protected static DocumentationFormat GetFormat(string text) =>
            text switch
            {
                "txt" => DocumentationFormat.Text,
                "md" => DocumentationFormat.MarkDown,
                "html" => DocumentationFormat.Html,
                _ => throw new ArgumentException($"Invalid format [ {text} ] format must be either txt, md, or html.")
            };
    }
}
