using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Generators
{
    public class ConsoleReporter :
        IProgress<string>
    {
        public void Report(string value)
        {
            Console.WriteLine(value);
        }
    }
}
