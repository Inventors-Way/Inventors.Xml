using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Analysis
{
    public abstract class Analyser
    {
        public Analyser(ObjectDocument document, Reporter reporter) 
        { 
            Document = document;    
            Reporter = reporter;
        }

        public ObjectDocument Document { get; }

        protected Reporter Reporter { get; }

        public static string SanitizeXSDName(string name) =>
            name.Replace("+", ".");
    }
}
