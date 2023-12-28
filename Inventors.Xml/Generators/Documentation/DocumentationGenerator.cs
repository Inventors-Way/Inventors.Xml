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
        public DocumentationGenerator(ObjectDocument document) 
        {
            this.document = document;
        }

        public void Run()
        {

            document.Run(this);
        }

        public void Visit(ClassElement element)
        {
        }

        public void Visit(EnumElement element)
        {
        }

        public void Visit(ArrayElement element) { }
        public void Visit(ChoiceElement element) { }
        public void Visit(NullElement element) { }

        private readonly ObjectDocument document;
    }
}
