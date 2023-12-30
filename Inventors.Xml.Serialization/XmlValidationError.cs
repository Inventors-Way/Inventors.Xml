using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;

namespace Inventors.Xml.Serialization
{
    public class XmlValidationError
    {
        public void Add(ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
            {
                _failed = true;
                _errors.Add(e.Message);
            }

            if (e.Severity == XmlSeverityType.Warning)
            {
                _warnings.Add(e.Message);
            }
        }

        public IList<string> Errors => _errors;

        public IList<string> Warnings => _warnings;

        public bool Failed => _failed;

        private bool _failed = false;
        private readonly List<string> _errors = new List<string>();
        private readonly List<string> _warnings = new List<string>();
    }
}
