using Inventors.Xml.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Generators.Documentation
{
    public class DocumentationGenerator : IElementVisitor
    {
        public DocumentationGenerator(ObjectDocument document, DocumentationSource source) 
        {
            this.document = document;
            this.source = source;
            reporter = new NullReporter();
        }

        public void Run() =>
            Run(new ConsoleReporter());

        public void Run(IProgress<string> reporter)
        {
            this.reporter = reporter;
            reporter.Report($"Generating documentation:");

            document.Run(this);
        }

        public void Visit(ClassElement element)
        {
            var path = source.GetElementPath(element.Name);
            var name = source.GetElementName(element.Name);

            reporter.Report("");
            reporter.Report($"CLASS ELEMENT: {Path.Combine(path, name)}.md");
        }

        public void Visit(EnumElement element)
        {
        }

        public void Visit(ArrayElement element) { }
        public void Visit(ChoiceElement element) { }
        public void Visit(NullElement element) { }

        private readonly ObjectDocument document;
        private readonly DocumentationSource source;
        private IProgress<string> reporter;
    }
}
