using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Test
{
    [TestClass]
    public class TypeExtensionsTests
    {
        public abstract class MyBaseClass
        {
            [XmlAttribute("id")]
            public string ID { get; set; }  = string.Empty;

            [XmlIgnore]
            public abstract int MyProperty { get; set; }
        }

        public class MyDerivedClass : MyBaseClass
        {
            [XmlAttribute("my-property")]
            public override int MyProperty { get; set; }

            [XmlAttribute("value")]
            public int Value { get; set; }
        }

        [TestMethod]
        public void AbstractBaseClassProperty()
        {
            Assert.IsFalse(typeof(MyDerivedClass).IsPropertyInherited(nameof(MyDerivedClass.MyProperty)));
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
    }
}
