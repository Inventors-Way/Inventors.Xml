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
using Throw;

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

        public void Run(string path, bool verbose)
        {
            Stopwatch stopwatch = new();

            try
            {
                Assembly = $"Loading assembly [ {AssemblyName} ]".Run(() => LoadAssembly(path));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed: {ex}");
                return;
            }

            foreach (var job in Jobs)
            {
                try
                {
                    Console.WriteLine();
                    Console.WriteLine($"Running: {job.Title}:");
                    stopwatch.Restart();
                    job.Run(path, this, verbose);
                    PrintRuntime(stopwatch);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"Error:");
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private static string GetName(string? fullname) =>
            fullname is null ? string.Empty : fullname.Split(',')[0];

        private Assembly LoadAssembly(string path)
        {
            if (AppDomain.CurrentDomain.GetAssemblies().Any(a => GetName(a.FullName) == AssemblyName))
                return Assembly.Load(AssemblyName);

            return Assembly.LoadFrom(GetAssemblyPath(path)
                .Throw()
                .IfFalse(filename => File.Exists(filename))
                .Value);
        }

        private string GetAssemblyPath(string path) =>
            string.IsNullOrEmpty(InputPath) ? 
            Path.Combine(path, AssemblyName) :
            Path.Combine(new string[]
                {
                    path,
                    InputPath,
                    $"{AssemblyName}.dll"
                });
        private static void PrintRuntime(Stopwatch stopwatch)
        {
            if (stopwatch.ElapsedMilliseconds > 999)
                Console.WriteLine($"Job completed in: {stopwatch.Elapsed.Seconds}s");
            else 
                Console.WriteLine($"Job completed in: {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
