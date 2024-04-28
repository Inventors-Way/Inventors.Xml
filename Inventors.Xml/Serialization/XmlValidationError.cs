using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;

namespace Inventors.Xml.Serialization
{
    public class XmlValidationError
    {
        public XmlValidationError(string typeName, bool warningsAsErrors) 
        { 
            this.typeName = typeName;
            this.warningsAsErrors = warningsAsErrors;
        }

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

                if (warningsAsErrors)
                    _failed = true;
            }
        }

        public IList<string> Errors => _errors;

        public IList<string> Warnings => _warnings;

        public bool Failed => _failed;

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"Validation result for type: {typeName} [ Passed: {!_failed}]");

            if (_errors.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("ERRORS");

                foreach (var e in _errors)
                    builder.AppendLine(e);

                builder.AppendLine();
            }

            if (_warnings.Count > 0)
            {
                if (_errors.Count == 0)
                    builder.AppendLine();

                builder.AppendLine("WARNINGS");

                foreach (var w in _warnings)
                    builder.AppendLine(w);

                builder.AppendLine();
            }

            return builder.ToString();
        }

        private bool _failed = false;
        private readonly List<string> _errors = new List<string>();
        private readonly List<string> _warnings = new List<string>();
        private readonly bool warningsAsErrors;
        private readonly string typeName;
    }
}
