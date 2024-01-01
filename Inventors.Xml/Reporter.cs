using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml
{
    public abstract class Reporter :
        IProgress<string>
    {
        private readonly bool _enabled = false;

        public Reporter(bool enabled) =>
            _enabled = enabled;

        public bool Enabled => _enabled;

        public void Report(string value)
        {
            if (Enabled)
                DoReporting(value);
        }

        protected abstract void DoReporting(string value);
    }
}
