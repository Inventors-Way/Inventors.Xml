using Inventors.Xml.Content;
using Inventors.Xml.Generators.Documentation;
using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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

            Console.Write("Parsing type ... ");
            var document = Inspector.Run(type);
            Console.WriteLine("done");

            Console.Write("Setting up documentation source ... ");
            var docPath = GetDocumentationPath(path, configuration);
            var source = DocumentationSource.Create(document, docPath)
                .SetInputFormat(DocumentationFileFormat)
                .Build();
            Console.WriteLine("done");
            Console.WriteLine("Generating documentation files:");
            var generator = new DocumentationGenerator(document, source);
            generator.Run();
        }
    }
}
