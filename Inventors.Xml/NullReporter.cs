using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml
{
    public class NullReporter :
        Reporter
    {
        public static NullReporter Instance { get; } = new NullReporter();

        public NullReporter() :
            base(false)
        {

        }

        protected override void DoReporting(string value)
        {            
        }
    }
}
