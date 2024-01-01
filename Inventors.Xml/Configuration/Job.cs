using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Throw;

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

        protected Type LoadType(IJobConfiguration configuration) => configuration.Assembly
                .ThrowIfNull()
                .Value
                .GetType(Type)
                .ThrowIfNull()
                .Value;

        protected static string GetDocumentationPath(string path, IJobConfiguration c) =>
            string.IsNullOrEmpty(c.DocumentationPath) ? path : Path.Combine(new string[]
                {
                    path,
                    c.DocumentationPath
                });

        protected static string GetOutputPath(string path, IJobConfiguration c) =>
            string.IsNullOrEmpty(c.OutputPath) ? path : Path.Combine(new string[]
                {
                    path,
                    c.OutputPath
                });

        public abstract void Run(string path, IJobConfiguration configuration, bool verbose = false);
    }
}
