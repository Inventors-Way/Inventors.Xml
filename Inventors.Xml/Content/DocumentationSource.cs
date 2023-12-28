using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Content
{
    public record ElementDocumentationInfo(string Path, string Name, DocumentationFormat Format);

    public static class ElementDocumentationInfoExtensions
    {
        public static string GetFilename(this ElementDocumentationInfo info) =>
            info.Format switch
            {
                DocumentationFormat.Text => $"{Path.Combine(info.Path, info.Name)}.txt",
                DocumentationFormat.MarkDown => $"{Path.Combine(info.Path, info.Name)}.md",
                DocumentationFormat.Html => $"{Path.Combine(info.Path, info.Name)}.html",
                _ => throw new NotSupportedException()
            };

        public static string GetFilename(this ElementDocumentationInfo info, string propertyName) =>
            info.Format switch
            {
                DocumentationFormat.Text => $"{Path.Combine(info.Path, info.Name)}.{propertyName}.txt",
                DocumentationFormat.MarkDown => $"{Path.Combine(info.Path, info.Name)}.{propertyName}.md",
                DocumentationFormat.Html => $"{Path.Combine(info.Path, info.Name)}.{propertyName}.html",
                _ => throw new NotSupportedException()
            };
    }

    public enum DocumentationFormat
    {
        Text,
        MarkDown,
        Html
    }

    public class DocumentationSource
    {
        public class DocumentationSourceOptions
        {
            internal DocumentationSourceOptions(ObjectDocument document, string path)
            {
                this.document = document;
                this.path = path;
            }

            public DocumentationSourceOptions SetOutputFormat(DocumentationFormat format) 
            {
                outputFormat = format;
                return this;
            }

            public DocumentationSourceOptions SetInputFormat(DocumentationFormat format)
            {
                inputFormat = format;
                return this;
            }

            public DocumentationSourceOptions SetCharacterData(bool enable = true)
            {
                cdata = enable;
                return this;
            }

            public DocumentationSourceOptions SetEncoding(bool enable = true)
            {
                encoding = enable;
                return this;
            }

            public ObjectDocument Document => document;

            public DocumentationFormat InputFormat => inputFormat;

            public DocumentationFormat OutputFormat => outputFormat;

            public string Path => path;

            public int PathOffset => pathOffset;

            public bool CDATA => cdata;

            public bool Encoding => encoding;

            private static int GetPathOffset(ObjectDocument document)
            {
                var parts = document.Namespace.Split('.');
                return parts.Length;
            }

            public DocumentationSource Build()
            {
                pathOffset = GetPathOffset(document);
                return new DocumentationSource(this);
            }

            private DocumentationFormat inputFormat;
            private DocumentationFormat outputFormat;
            private readonly ObjectDocument document;
            private readonly string path;
            private int pathOffset;
            private bool cdata = false;
            private bool encoding = false;
        }

        public static DocumentationSourceOptions Create(ObjectDocument document, string path) => new (document, path);

        private DocumentationSource(DocumentationSourceOptions options)
        {
            if (!Directory.Exists(options.Path))
                throw new ArgumentException($"Basepath [ {options.Path} ] does not exists", nameof(options));

            this.options = options;

            markdownPipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
        }

        public DocumentationFormat InputFormat => options.InputFormat;

        public DocumentationFormat OutputFormat => options.OutputFormat;

        public string[] GetPaths(string name)
        {
            var parts = name.Split('.');

            if (parts.Length < options.PathOffset + 1)
                throw new ArgumentException($"Invalid element name: {name}");

            if (parts.Length == options.PathOffset + 1)
                return new string[] { options.Path };

            string[] retValue = new string[parts.Length - options.PathOffset];

            retValue[0] = options.Path;

            for (int i = options.PathOffset; i < parts.Length - 1; ++i)
            {
                retValue[i - options.PathOffset + 1] = parts[i];
            }

            return retValue;
        }

        public string GetElementPath(string name)
        {
            var parts = GetPaths(name);

            if (parts.Length == 0)
                return options.Path;

            return Path.Combine(parts);
        }

        public static string GetElementName(string name)
        {
            var parts = name.Split('.');

            if (parts.Length == 0)
                throw new ArgumentException($"Invalid element name: {name}");

            return parts[^1];
        }

        public ElementDocumentationInfo GetElement(string name) =>
            new(Path: GetElementPath(name), Name: GetElementName(name), Format: InputFormat);

        private string Format(string text)
        {
            if (options.Encoding)
            {
                text = WebUtility.HtmlEncode(text);
            }

            if (options.CDATA)
            {
                text = $"<![CDATA[{text}]]>";
            }

            return text;
        }

        public string this[string? filename]
        {
            get
            {
                if (filename is null)
                    return string.Empty;

                if (File.Exists(filename))
                {
                    var text = File.ReadAllText(filename);

                    if (string.IsNullOrEmpty(text))
                        return string.Empty;

                    switch (InputFormat)
                    {
                        case DocumentationFormat.Text:
                            return Format(text);
                        case DocumentationFormat.Html:
                            return Format(text);
                        case DocumentationFormat.MarkDown:
                            {
                                switch (OutputFormat)
                                {
                                    case DocumentationFormat.Text:
                                        return Format(Markdown.ToPlainText(text));
                                    case DocumentationFormat.Html:
                                        return Format(Markdown.ToHtml(text).Trim());
                                    case DocumentationFormat.MarkDown:
                                        return Format(text);
                                    default:
                                        return Format(text);
                                }
                            }
                        default:
                            throw new NotSupportedException();
                    }
                }

                return string.Empty;
            }
        }

        private readonly DocumentationSourceOptions options;
        private readonly MarkdownPipeline markdownPipeline;
    }
}
