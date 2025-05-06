using Inventors.Xml.Serialization;
using Inventors.Xml.Test.TestObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Test
{
    [TestClass]
    public class DeserializationTest
    {
        [TestMethod]
        public void T01_DeserializeObjectHirachy()
        {
            var text = File.ReadAllText(Path.Combine(IntegrationTests.DataDirectory, "final-object.xml"));
            var data = text.ToObject<FinalClass>();

            Console.WriteLine(data);

        }
    }
}
