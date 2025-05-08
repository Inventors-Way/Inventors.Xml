using Inventors.Xml.Configuration;
using Inventors.Xml.Content;
using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Throw;

namespace Inventors.Xml
{
    [XmlRoot("xsdg")]
    [XmlDocumentation("@XSDGConfig.md")]
    public class XSDGConfig :
        IJobConfiguration
    {
        [XmlAttribute("assembly")]
        [XmlRequired]
        [XmlDocumentation("@XSDGConfig.AssemblyName.md")]
        public string AssemblyName { get; set; } = string.Empty;

        [XmlAttribute("documentation-assembly")]
        [XmlOptional]
        [XmlDocumentation("Assembly for the documentation generator")]
        public string DocumentationAssemblyName { get; set; } = string.Empty;

        [XmlAttribute("documentation-generator")]
        [XmlOptional]
        [XmlDocumentation("@XSDGConfig.DocumentationGenerator.md")]
        public string DocumentationGenerator { get; set; } = string.Empty;

        [XmlAttribute("output-path")]
        [XmlOptional]
        [XmlDocumentation("@XSDGConfig.OutputPath.md")]
        public string OutputPath { get; set; } = string.Empty;

        [XmlAttribute("input-path")]
        [XmlOptional]
        [XmlDocumentation("@XSDGConfig.InputPath.md")]
        public string InputPath { get; set; } = string.Empty;

        [XmlElement("schema", typeof(SchemaJob))]
        public List<Job> Jobs { get; } = new List<Job>();

        [XmlIgnore]
        public Assembly? Assembly { get; private set; }

        [XmlIgnore]
        public Assembly? DocumentationAssembly { get; private set; }

        public void Run(string path, bool verbose)
        {
            Stopwatch stopwatch = new();
            IDocumentationSource? docGenerator = null;

            try
            {
                Assembly = $"Loading assembly [ {AssemblyName} ]".Run(() => LoadAssembly(path, AssemblyName));
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed!");
                Console.WriteLine($" {ex}");
                return;
            }

            try
            {
                if (!string.IsNullOrEmpty(DocumentationGenerator) && !string.IsNullOrEmpty(DocumentationAssemblyName))
                    docGenerator = "Creating documentation generator: ...".Run(() => CreateDocumentationGenerator(path));
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed!");
                Console.WriteLine($" {ex}");
                return;
            }

            foreach (var job in Jobs)
            {
                try
                {
                    Console.WriteLine();
                    Console.WriteLine($"Running: {job.Title}:");

                    stopwatch.Restart();

                    job.Run(path, this, docGenerator, verbose);

                    Console.WriteLine();
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

        private Assembly LoadAssembly(string path, string assemblyName)
        {
            if (AppDomain.CurrentDomain.GetAssemblies().Any(a => GetName(a.FullName) == assemblyName))
                return Assembly.Load(assemblyName);

            var assemblyPath = GetAssemblyPath(path, assemblyName);

            if (!File.Exists(assemblyPath))
                throw new InvalidOperationException($"Did not find assembly [ {assemblyPath} ]");

            return Assembly.LoadFrom(assemblyPath);
        }

        private IDocumentationSource CreateDocumentationGenerator(string path)
        {
            DocumentationAssembly = $"Loading documentation assembly [ {DocumentationAssemblyName} ]".Run(() => LoadAssembly(path, DocumentationAssemblyName));

            if (DocumentationAssembly is null)
                throw new InvalidOperationException($"Assembly {DocumentationAssemblyName} has not been loaded");

            if (DocumentationAssembly.GetType(DocumentationGenerator) is not Type type)
                throw new InvalidOperationException($"Failed to get type {DocumentationGenerator} for the documentation generator");

            if (Activator.CreateInstance(type) is not IDocumentationSource source)
                throw new InvalidOperationException("Documentation generator does not implement the IDocumentationSource interface");

            return source;
        }

        private string GetAssemblyPath(string path, string name) =>
            Path.Combine(new string[]
                {
                    path,
                    InputPath,
                    $"{name}.dll"
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
