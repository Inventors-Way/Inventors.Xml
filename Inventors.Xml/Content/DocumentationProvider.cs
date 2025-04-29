using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Throw;

namespace Inventors.Xml.Content
{
    public record ElementDocumentationInfo(string IDDocumentationFormat, DocumentationFormat Format);

    public enum DocumentationFormat
    {
        [XmlEnum("text")]
        Text,
        [XmlEnum("markdown")]
        MarkDown,
        [XmlEnum("html")]
        Html
    }

    public class DocumentationProvider
    {
        public class DocumentationSourceOptions
        {
            internal DocumentationSourceOptions(ObjectDocument document, IDocumentationSource? source)
            {
                this.document = document;
                this.source = source is null ? new NullDocumentationSource() : source;
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

            public IDocumentationSource Source => source;

            public bool CDATA => cdata;

            public bool Encoding => encoding;

            public DocumentationProvider Build() => new DocumentationProvider(this);

            private DocumentationFormat inputFormat;
            private DocumentationFormat outputFormat;
            private readonly ObjectDocument document;
            private readonly IDocumentationSource source;
            private bool cdata = false;
            private bool encoding = false;
        }

        public static DocumentationSourceOptions Create(ObjectDocument document, IDocumentationSource? source) => new (document, source);

        private DocumentationProvider(DocumentationSourceOptions options)
        {
            this.options = options;

            markdownPipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
        }

        public DocumentationFormat InputFormat => options.InputFormat;

        public DocumentationFormat OutputFormat => options.OutputFormat;

        private string Format(string text)
        {
            if (options.Encoding)
                text = WebUtility.HtmlEncode(text);

            if (options.CDATA)
                text = $"<![CDATA[{text}]]>";

            return text;
        }

        public string this[string? id]
        {
            get
            {
                if (string.IsNullOrEmpty(id))
                    return string.Empty;

                var text = options.Source.GetItem(id);

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
        }

        private readonly DocumentationSourceOptions options;
        private readonly MarkdownPipeline markdownPipeline;
    }
}
