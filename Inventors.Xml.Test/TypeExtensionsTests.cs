using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Test
{
    [TestClass]
    public class TypeExtensionsTests
    {
        public class Content
        {
            [XmlAttribute("text")]
            public string Text { get; set; } = string.Empty;
        }

        public class RatedContent : Content
        {
            [XmlAttribute("rating")]
            public int Rating { get; set; } = 0;
        }

        public abstract class MyBaseClass
        {
            [XmlAttribute("id")]
            public string ID { get; set; }  = string.Empty;

            [XmlIgnore]
            public abstract int IgnoredProperty { get; set; }

            [XmlElement("description")]
            [XmlRequired]
            public Content? Description { get; set; }
        }

        public class MyDerivedClass : MyBaseClass
        {
            [XmlAttribute("my-property")]
            public override int IgnoredProperty { get; set; }

            [XmlAttribute("value")]
            public int Value { get; set; }

            [XmlElement("review", typeof(Content))]
            [XmlElement("rated-review", typeof(RatedContent))]
            [XmlOptional]
            public Content? Review { get; set; }

            [XmlArray("comments")]
            [XmlArrayItem("standard-comment", typeof(Content))]
            [XmlArrayItem("rated-comment", typeof(RatedContent))]
            public List<Content> Comments { get; } = new List<Content>();
        }

        [TestMethod]
        public void AbstractBaseClassProperty()
        {
            Assert.IsFalse(typeof(MyDerivedClass).IsPropertyInherited(nameof(MyDerivedClass.IgnoredProperty)));
        }

        [TestMethod]
        public void BaseClassProperty()
        {
            Assert.IsTrue(typeof(MyDerivedClass).IsPropertyInherited(nameof(MyDerivedClass.ID)));
        }

        [TestMethod]
        public void DerivedClassProperty()
        {
            Assert.IsFalse(typeof(MyDerivedClass).IsPropertyInherited(nameof(MyDerivedClass.Value)));
        }

        [TestMethod]
        public void AnalyseClass()
        {
            Type type = typeof(MyDerivedClass);

            foreach (var property in type.GetProperties())
            {
                Console.WriteLine($"{property.Name}: {property.GetSchemaType(property.PropertyType)} [ Inherited: {type.IsPropertyInherited(property.Name)}]");
            }
        }
    }
}
