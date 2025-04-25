using Inventors.Xml.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Inventors.Xml.Generators.Documentation
{
    public class DocumentationGenerator : IElementVisitor
    {
        public DocumentationGenerator(ObjectDocument document, DocumentationSource source, Reporter reporter) 
        {
            this.document = document;
            this.source = source;
            this.reporter = reporter;
        }

        public void Run()
        {
            reporter.Report($"Generating documentation [ {document.Root.Name} ]");

            document.Run(this);
        }

        public void Visit(ClassElement element)
        {
            var info = Configure(element.Name);
            Checkfile(info.GetFilename());

            foreach (var e in element.Elements)
            {
                if (!e.Type.IsNested)
                {
                    Checkfile(info.GetFilename(e.PropertyName));
                }
            }

            foreach (var a in element.Attributes)
            {
                Checkfile(info.GetFilename(a.PropertyName));
            }
        }

        public void Visit(EnumElement element)
        {
            var info = Configure(element.Name);
            Checkfile(info.GetFilename());

            foreach (var value in element.Values)
            {
                Checkfile(info.GetFilename(value.Name));
            }
        }

        private ElementDocumentationInfo Configure(string name)
        {
            reporter.Report($"Checking: {name}");
            CreatePath(name);
            return source.GetElement(name);
        }

        private void Checkfile(string filename)
        {
            if (!File.Exists(filename))
            {
                File.WriteAllText(filename, "");
                reporter.Report($"- file: {filename}");
            }
        }

        private void CreatePath(string name)
        {
            var paths = source.GetPaths(name);
            var current = paths[0];

            for (int n = 1; n < paths.Length; ++n)
            {
                current = Path.Combine(current, paths[n]);

                if (!Directory.Exists(current))
                {
                    Directory.CreateDirectory(current);
                    reporter.Report($"- path: {current}");
                }
                else
                {
                    /*
                    var files = GetFiles(current);

                    foreach (var file in files)
                    {
                        FileInfo fileInfo = new (file);
                        
                        if (fileInfo.Length == 0)
                            File.Delete(file);  
                    }*/
                }
            }
        }

        private string[] GetFiles(string path)
        {
            switch (source.InputFormat)
            {
                case DocumentationFormat.MarkDown:
                    return Directory.GetFiles(path, "*.md");
                case DocumentationFormat.Text:
                    return Directory.GetFiles(path, "*.txt");
                case DocumentationFormat.Html:
                    return Directory.GetFiles(path, "*.html");
            }

            return Array.Empty<string>();
        }

        public void Visit(ArrayElement element) { }
        public void Visit(ChoiceElement element) { }
        public void Visit(NullElement element) { }

        private readonly ObjectDocument document;
        private readonly DocumentationSource source;
        private Reporter reporter;
    }
}
