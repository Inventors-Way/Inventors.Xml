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
    [XmlDocumentation(@"
# XSDG Configuration File

The xsdg tool is controlled by a configuration file, which specifies:

1. Defines paths to the Assembly, Documentation Files, and the Output Path for XSD Schemas. All paths within the configuration are relative to the working directory of the XSDG tool. 
2. The name of the Assembly that contains the types to be analyzed.
3. A number of jobs to be performed. Two jobs can be configured:
	1. Documentation jobs: which will generate templates for Documentation Files.
	2. Schema jobs: which will generate XSD Schemas for selected types.

This configuration file is passed to the xsdg tool as a parameter:

```
xsdg -p [working directory] [name of configuration file]
```

The ```-p [working directory]``` is optional and if specified will set the working directory of the xsdg tool. If it is not specied the working directory will be the directory from where the xsdg tool is invoked. 
")]
    public class XSDGConfig :
        IJobConfiguration
    {
        [XmlAttribute("assembly")]
        [XmlRequired]
        [XmlDocumentation(@"
# Assembly Name

Name of the Assembly from which to load types. It is specified without the file extension of the assembly, which is assumed to be ```dll```.
")]
        public string AssemblyName { get; set; } = string.Empty;

        [XmlAttribute("documentation-path")]
        [XmlOptional]
        [XmlDocumentation(@"
# Path to XSD documentation

This attribute specifies the relative path to the Documentation Files that will be included in ```xsd:documention``` elements in generated XSD Schemas, or where templates for the Documentation Files will be generated. This attribute is optional. If it is not specified then the Documentation Files will be presumed located in the current working directory.

Please note, this path must not start with a path separator, as this will cause the working directory path to be discarded; most likely cause an error when running the program.
")]
        public string DocumentationPath { get; set; } = string.Empty;

        [XmlAttribute("output-path")]
        [XmlOptional]
        [XmlDocumentation(@"
# Output path for generated XSD Schemas

This attribute specifies the relative path to which generated XSD Schemas will be written. This attribute is optional. If it is not specified then XSD Schemas will be written to the current working directory.

Please note, this path must not start with a path separator, as this will cause the working directory path to be discarded; most likely cause an error when running the program.
")]
        public string OutputPath { get; set; } = string.Empty;

        [XmlAttribute("input-path")]
        [XmlOptional]
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
