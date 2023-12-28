using Inventors.Xml.Generators.Xsd;
using Inventors.Xml.Test.TestObjects;

namespace Inventors.Xml.Test
{
    [TestClass]
    public class IntegrationTests
    {
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

            Console.WriteLine(content);
            WriteSchema("company.xsd", content);
        }


        private void WriteSchema(string filename, string content)
        {
            var path = Directory.GetCurrentDirectory();
            File.WriteAllText($"{path}\\..\\..\\..\\TestData\\{filename}", content);
        }
    }
}