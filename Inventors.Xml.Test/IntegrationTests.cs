using Inventors.Xml.Content;
using Inventors.Xml.Generators.Xsd;
using Inventors.Xml.Test.TestObjects;
using Inventors.Xml.Serialization;
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

        public static string GetData(string filename) => File.ReadAllText(Path.Combine(DataDirectory, filename));

        public static string DocumentationDirectory => Path.Combine(ProjectDir, "TestDocumentation");

        public static string DisposableDocumentationDirectory => Path.Combine(ProjectDir, "TestDocumentationDisposable");

        [TestMethod]
        public void T01_ObjectDocument()
        {
            var document = ObjectDocument.Parse(typeof(Company), NullReporter.Instance);

            Console.WriteLine(document);
        }

        [TestMethod]
        public void T02_GenerateSchema()
        {
            var document = ObjectDocument.Parse(typeof(Company), NullReporter.Instance);
            DocumentationProvider documentation = DocumentationProvider.Create(document, new XSDGConfigDocumentation())
                .SetInputFormat(DocumentationFormat.MarkDown)
                .SetOutputFormat(DocumentationFormat.Html)
                .SetEncoding(true)
                .Build();

            var generator = new XSDGenerator(document, documentation);
            var content = generator.Run();

            File.WriteAllText(Path.Combine(DataDirectory, generator.FileName), content);

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
        public void T07_GenerateXsdgSchema()
        {
            var reporter = new ConsoleReporter(true);
            var document = ObjectDocument.Parse(typeof(XSDGConfig), reporter);
            var generator = new XSDGenerator(document);
            var content = generator.Run();

            File.WriteAllText(Path.Combine(DataDirectory, generator.FileName), content);

            Console.WriteLine(content);
        }

        [TestMethod]
        public void T08_ValidationTestPassingValidation()
        {
            var xsdSchema = typeof(Company).GetSchema(); 

            Company acmeCompany = GetData("AcmeCorp.xml").ToObject<Company>(xsdSchema)
                .OnSuccess(company => Console.WriteLine($"Loaded company {company.Name}"))
                .OnError(errors => Assert.IsTrue(false)); // We should not get an error
        }

        [TestMethod]
        public void T09_ValidationTestFailingValidation()
        {
            var xsdSchema = typeof(Company).GetSchema();

            var result = GetData("InvalidCompany.xml").ToObject<Company>(xsdSchema)
                .OnSuccess(company => Assert.IsTrue(false)) // We should not have a success
                .OnError(errors => Console.WriteLine($"{errors}"));
        }
    }
}