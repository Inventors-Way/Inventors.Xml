using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Test.TestObjects
{
    public class A
    {
        [XmlAttribute("a-factor")]
        public int Afactor { get; set; }

        public override string ToString() => $"A {Afactor}";
    }

    public class  B
    {
        [XmlAttribute("b-factor")]
        public int Bfactor { get; set; }

        public override string ToString() => $"B {Bfactor}";
    }

    public class C
    {
        [XmlAttribute("c-factor")]
        public int Cfactor { get; set; }

        public override string ToString() => $"C {Cfactor}";
    }

    public class D
    {
        [XmlAttribute("d-factor")]
        public int Dfactor { get; set; }

        public override string ToString() => $"D {Dfactor}";
    }

    public class E
    {
        [XmlAttribute("e-factor")]
        public int Efactor { get; set; }

        public override string ToString() => $"E {Efactor}";
    }

    public class F
    {
        [XmlAttribute("f-factor")]
        public int Ffactor { get; set; }

        public override string ToString() => $"F {Ffactor}";
    }

    [XmlRoot("final-class")]
    public class FinalClass : BaseClassB
    {
        [XmlElement("d")]
        public D? D { get; set; } 

        [XmlElement("f")]
        public F? F { get; set; } 
    }

    public abstract class BaseClassB : BaseClassA
    {
        [XmlAttribute("value")]
        public int Value { get; set; }

        [XmlElement("b")]
        public B? B { get; set; } 

        [XmlElement("c")]
        public C? C { get; set; } 
    }

    public abstract class BaseClassA
    {
        [XmlAttribute("name")]
        [XmlRequired]
        public string Name { get; set; } = string.Empty;

        [XmlElement("a")]
        public A? A { get; set; } 
    }
}
