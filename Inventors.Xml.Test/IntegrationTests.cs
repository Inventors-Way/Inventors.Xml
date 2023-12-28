using Inventors.Xml.Content;
using Inventors.Xml.Generators.Documentation;
using Inventors.Xml.Generators.Xsd;
using Inventors.Xml.Test.TestObjects;
using Newtonsoft.Json.Bson;
using System.IO;

namespace Inventors.Xml.Test
{
    [TestClass]
    public class IntegrationTests
    {
        public static string ProjectDir
        {
            get
            {
                var p = Directory.GetCurrentDirectory();
                p = Directory.GetParent(p)?.FullName ?? throw new InvalidOperationException("null");
                p = Directory.GetParent(p)?.FullName ?? throw new InvalidOperationException("null");
                p = Directory.GetParent(p)?.FullName ?? throw new InvalidOperationException("null");

                return p;
            }
        }

        public static string DataDirectory => $"{Directory.GetCurrentDirectory()}\\..\\..\\..\\TestData\\";

        public static string DocumentationDirectory => Path.Combine(ProjectDir, "TestDocumentation");

        public static string DisposableDocumentationDirectory => Path.Combine(ProjectDir, "TestDocumentationDisposable");

        [TestMethod]
        public void T01_ObjectDocument()
        {
            var document = Inspector.Run(typeof(Company));

            Console.WriteLine(document);
        }

        [TestMethod]
        public void T02_GenerateSchema()
        {
            var document = Inspector.Run(typeof(Company));
            var documentation = DocumentationSource.Create(document, DocumentationDirectory)
                .SetInputFormat(DocumentationFormat.MarkDown)
                .SetOutputFormat(DocumentationFormat.Html)
                .SetEncoding(true)
                .Build();

            var generator = new XSDGenerator(document, documentation);
            var content = generator.Run();

            File.WriteAllText(Path.Combine(DataDirectory, "company.xsd"), content);

            Console.WriteLine(content);
        }

        [TestMethod]
        public void T03_LoadData()
        {
            var text = File.ReadAllText(Path.Combine(DataDirectory, "AcmeCorp.xml"));
            var company = text.ToObject<Company>();

            Console.WriteLine(company);
        }

        [TestMethod]
        public void T04_GenerateMarkdownDocumentation()
        {
            var document = Inspector.Run(typeof(Company));
            var source = DocumentationSource.Create(document, DocumentationDirectory)
                .SetInputFormat(DocumentationFormat.MarkDown)
                .SetOutputFormat(DocumentationFormat.Html)
                .Build();
            var generator = new DocumentationGenerator(document, source);
            generator.Run();
        }

        [TestMethod]
        public void T05_GenerateTextDocumentation()
        {
            var document = Inspector.Run(typeof(Company));
            var source = DocumentationSource.Create(document, DocumentationDirectory)
                .SetInputFormat(DocumentationFormat.Text)
                .SetOutputFormat(DocumentationFormat.Html)
                .Build();
            var generator = new DocumentationGenerator(document, source);
            generator.Run();
        }

        [TestMethod]
        public void T06_GenerateDisposableDocumentation()
        {
            if (Directory.Exists(DisposableDocumentationDirectory))
            {
                Directory.Delete(DisposableDocumentationDirectory, true);
                Console.WriteLine($"Deleted: {DisposableDocumentationDirectory}");
            }

            Directory.CreateDirectory(DisposableDocumentationDirectory);

            var document = Inspector.Run(typeof(Company));
            var source = DocumentationSource.Create(document, DisposableDocumentationDirectory)
                .SetInputFormat(DocumentationFormat.MarkDown)
                .SetOutputFormat(DocumentationFormat.Text)
                .Build();
            var generator = new DocumentationGenerator(document, source);
            generator.Run();
        }
    }
}