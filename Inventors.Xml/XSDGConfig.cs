using Inventors.Xml.Configuration;
using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml
{
    [XmlRoot("xsdg")]
    public class XSDGConfig :
        IJobConfiguration
    {
        [XmlAttribute("assembly")]
        [XmlRequired(true)]
        public string AssemblyName { get; set; } = string.Empty;

        [XmlAttribute("documentation-path")]
        [XmlRequired(false)]
        public string DocumentationPath { get; set; } = string.Empty;

        [XmlAttribute("output-path")]
        [XmlRequired(false)]
        public string OutputPath { get; set; } = string.Empty;

        [XmlAttribute("input-path")]
        [XmlRequired(false)]
        public string InputPath { get; set; } = string.Empty;

        [XmlElement("documentation", typeof(DocumentationJob))]
        [XmlElement("schema", typeof(SchemaJob))]
        public List<Job> Jobs { get; } = new List<Job>();

        [XmlIgnore]
        public Assembly? Assembly { get; private set; }

        public void Run(string path)
        {
            Stopwatch stopwatch = new();

            try
            {
                Assembly = $"Loading assembly [ {AssemblyName} ]".Run(() => LoadAssembly(path));
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed");
                Console.WriteLine(ex);
                return;
            }

            foreach (var job in Jobs)
            {
                try
                {
                    Console.WriteLine();
                    Console.WriteLine($"Running: {job.Title}:");
                    stopwatch.Restart();
                    job.Run(path, this);
                    PrintRuntime(stopwatch);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private static string GetName(string? fullname)
        {
            if (fullname is null)
                return string.Empty;

            var parts = fullname.Split(',');
            return parts[0];
        }

        private Assembly LoadAssembly(string path)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            if (assemblies.Any(a => GetName(a.FullName) == AssemblyName))
                return Assembly.Load(AssemblyName);

            var filename = GetAssemblyPath(path);

            if (!File.Exists(filename))
            {
                throw new ArgumentException($"Assembly not found [ {filename} ]");
            }

            return Assembly.LoadFrom(filename);
        }

        private string GetAssemblyPath(string path)
        {
            if (string.IsNullOrEmpty(InputPath))
            {
                return Path.Combine(path, AssemblyName);
            }
            else
            {
                return Path.Combine(new string[]
                {
                    path,
                    InputPath,
                    $"{AssemblyName}.dll"
                });
            }
        }

        private static void PrintRuntime(Stopwatch stopwatch)
        {
            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                Console.WriteLine($"Job completed in: {stopwatch.Elapsed.Seconds}s");
            }
            else 
            {
                Console.WriteLine($"Job completed in: {stopwatch.ElapsedMilliseconds}ms");
            }
        }
    }
}
