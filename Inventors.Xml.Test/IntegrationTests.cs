using Inventors.Xml.Generators.Xsd;
using Inventors.Xml.Test.TestObjects;
using Newtonsoft.Json.Bson;
using System.IO;

namespace Inventors.Xml.Test
{
    [TestClass]
    public class IntegrationTests
    {
        public static string DataDirectory => $"{Directory.GetCurrentDirectory()}\\..\\..\\..\\TestData\\";

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
    }
}