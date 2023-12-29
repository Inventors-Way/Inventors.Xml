using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Configuration
{
    public abstract class Job
    {
        [XmlAttribute("title")]
        [XmlRequired(true)]
        public string Title { get; set; } = string.Empty;

        [XmlAttribute("type")]
        [XmlRequired(true)]
        public string Type { get; set; } = string.Empty;

        protected Type LoadType(string path, IJobConfiguration configuration)
        {
            if (configuration.Assembly is null)
                throw new InvalidOperationException("No assembly loaded");

            var type = configuration.Assembly.GetType(Type);

            return type ?? throw new InvalidOperationException($"Failed to load type [ {Type} ]");
        }

        protected static string GetDocumentationPath(string path, IJobConfiguration c)
        {
            if (string.IsNullOrEmpty(c.DocumentationPath))
            {
                return path;
            }
            else
            {
                return Path.Combine(new string[]
                {
                    path,
                    c.DocumentationPath
                });
            }
        }

        protected static string GetOutputPath(string path, IJobConfiguration c)
        {
            if (string.IsNullOrEmpty(c.OutputPath))
            {
                return path;
            }
            else
            {
                return Path.Combine(new string[]
                {
                    path,
                    c.OutputPath
                });
            }
        }

        public abstract void Run(string path, IJobConfiguration configuration);
    }
}
