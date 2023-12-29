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

        private static string GetAssemblyPath(string path, IJobConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.InputPath))
            {
                return Path.Combine(path, configuration.Assembly);
            }
            else
            {
                return Path.Combine(new string[]
                {
                    path,
                    configuration.InputPath,
                    configuration.Assembly
                });
            }
        }

        protected Type LoadType(string path, IJobConfiguration configuration)
        {
            // Yes I know it is an ugly hack! But as our dear PM said: Live With It!
            if (configuration.Assembly == "Inventors.Xml.dll")
            {
                var assembly = typeof(XSDGConfig).Assembly;
                var type = assembly.GetType(Type);
                return type ?? throw new InvalidOperationException($"Failed to load type [ {Type} ]");
            }
            else
            {
                var assemblyPath = GetAssemblyPath(path, configuration);

                if (!File.Exists(assemblyPath))
                {
                    Console.WriteLine($"Did not find assembly: {assemblyPath}");
                    throw new ArgumentException("Assembly not found");
                }

                var assembly = Assembly.LoadFrom(assemblyPath);
                var type = assembly.GetType(Type);

                return type ?? throw new InvalidOperationException($"Failed to load type [ {Type} ]");
            }
        }

        public abstract void Run(string path, IJobConfiguration configuration);
    }
}
