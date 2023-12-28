using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Content
{
    public class DocumentationSource
    {
        public DocumentationSource(string basePath, ObjectDocument document) 
        {
            if (!Directory.Exists(basePath))
                throw new ArgumentException($"Basepath [ {basePath} ] does not exists", nameof(basePath));

            this.basePath = basePath;
            pathOffset = GetPathOffset(document);
        }

        private int GetPathOffset(ObjectDocument document) 
        {
            var parts = document.Namespace.Split('.');
            return parts.Length;
        }

        public string[] GetPaths(string name)
        {
            var parts = name.Split('.');

            if (parts.Length < pathOffset + 1)
                throw new ArgumentException($"Invalid element name: {name}");

            if (parts.Length == pathOffset + 1)
                return new string[] { basePath };

            string[] retValue = new string[parts.Length - pathOffset];

            retValue[0] = basePath;

            for (int i = pathOffset; i < parts.Length - 1; ++i)
            {
                retValue[i - pathOffset + 1] = parts[i];
            }

            return retValue;
        }

        public string GetElementPath(string name)
        {
            var parts = GetPaths(name);

            if (parts.Length == 0)
                return basePath;

            return Path.Combine(parts);
        }

        public string GetElementName(string name) 
        {
            var parts = name.Split('.');

            if (parts.Length == 0)
                throw new ArgumentException($"Invalid element name: {name}");

            return parts[^1];
        }

        private readonly string basePath;
        private readonly int pathOffset;
    }
}
