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
            var type = $"Loading type: {Type}".Run(() => LoadType(configuration));
            var document = "Parsing type".Run(() => ObjectDocument.Parse(type));
            var docPath = GetDocumentationPath(path, configuration);
            var source = "Setting up documentation source".Run(() =>
                DocumentationSource.Create(document, docPath)
                    .SetInputFormat(DocumentationFileFormat)
                    .Build());
            var generator = "Creating documentation generator".Run(() => new DocumentationGenerator(document, source));

            generator.Run();
        }
    }
}
