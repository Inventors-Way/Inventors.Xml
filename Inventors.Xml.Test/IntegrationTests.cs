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
                var p0 = Directory.GetCurrentDirectory();
                var p1 = Directory.GetParent(p0);
                var p2 = Directory.GetParent(p1?.FullName ?? throw new InvalidOperationException("null"));
                var p3 = Directory.GetParent(p2?.FullName ?? throw new InvalidOperationException("null"));

                return p3?.FullName ?? throw new InvalidOperationException("null");
            }
        }

        public static string DataDirectory => $"{Directory.GetCurrentDirectory()}\\..\\..\\..\\TestData\\";

        public static string DocumentationDirectory => Path.Combine(ProjectDir, "TestDocumentation");

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
            var generator = new XSDGenerator(document);
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
        public void T04_GenerateDocumentation()
        {
            var document = Inspector.Run(typeof(Company));
            var source = new DocumentationSource(DocumentationDirectory, document);
            var generator = new DocumentationGenerator(document, source);
            generator.Run();
        }
    }
}