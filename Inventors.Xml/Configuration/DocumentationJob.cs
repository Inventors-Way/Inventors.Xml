using Inventors.Xml.Content;
using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Configuration
{
    public class DocumentationJob :
        Job
    {
        [XmlAttribute("documentation-file-format")]
        [XmlRequired(false)]
        public DocumentationFormat DocumentationFileFormat { get; set; } = DocumentationFormat.MarkDown;

        public override void Run(string path, IJobConfiguration configuration)
        {
            Console.Write($"Loading type: {Type} ... ");
            var type = LoadType(path, configuration);
            Console.WriteLine("done");

            Console.WriteLine("Documentation job");
        }
    }
}
