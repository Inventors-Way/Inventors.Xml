using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml
{
    public static class AssemblyExtensions
    {
        public static string GetEmbeddedString(this Assembly self, string resourceName)
        {
            using Stream? stream = self.GetManifestResourceStream(resourceName) ?? 
                throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }
    }
}
