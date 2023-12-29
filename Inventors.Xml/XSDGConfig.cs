using Inventors.Xml.Configuration;
using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public string Assembly { get; set; } = string.Empty;

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

        public void Run(string path)
        {
            Stopwatch stopwatch = new Stopwatch();

            foreach (var job in Jobs)
            {
                Console.WriteLine($"Running: {job.Title}:");
                stopwatch.Restart();
                job.Run(path, this);
                PrintRuntime(stopwatch);
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
