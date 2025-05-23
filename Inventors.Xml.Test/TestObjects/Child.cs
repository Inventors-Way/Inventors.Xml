﻿using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Test.TestObjects
{
    [XmlDocumentation("@Child")]
    public class Child
    {
        [XmlAttribute("name")]
        [XmlRequired]
        [XmlDocumentation("@Child.Name")]
        public string Name { get; set; } = string.Empty;

        [XmlAttribute("age")]
        [XmlOptional]
        [XmlDocumentation(@"Age of the child [ calculated.int ]")]
        public int Age { get; set; } = 0;

        public override string ToString() => $"Child: {Name}, Age: {Age}";

    }
}
