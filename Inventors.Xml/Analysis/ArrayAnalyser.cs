using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Analysis
{
    public class ArrayAnalyser : Analyser
    {
        public ArrayAnalyser(ObjectDocument document, Reporter reporter) : base(document, reporter)
        {
        }

        public string Analyse(string elementName, Type property)
        {
            throw new NotImplementedException();
        }
    }
}
