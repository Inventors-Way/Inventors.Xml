using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Analysis
{
    public class ChoiceAnalyser : Analyser
    {
        public ChoiceAnalyser(ObjectDocument document, Reporter reporter) : base(document, reporter)
        {
        }

        public string Analyse(Type property)
        {
            throw new NotImplementedException();
        }
    }
}
