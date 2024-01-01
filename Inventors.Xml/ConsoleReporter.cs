using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml
{
    public class ConsoleReporter :
        Reporter,
        IProgress<string>
    {
        public ConsoleReporter(bool verbose) :
            base(verbose)
        { }

        protected override void DoReporting(string value) => Console.WriteLine(value);
    }
}
