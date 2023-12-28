﻿using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
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
                _ => throw new NotSupportedException()
            };

        public static string GetFilename(this ElementDocumentationInfo info, string propertyName) =>
            info.Format switch
            {
                DocumentationFormat.Text => $"{Path.Combine(info.Path, info.Name)}.{propertyName}.txt",
                DocumentationFormat.MarkDown => $"{Path.Combine(info.Path, info.Name)}.{propertyName}.md",
                _ => throw new NotSupportedException()
            };
    }

    public enum DocumentationFormat
    {
        Text,
        MarkDown
    }

    public class DocumentationSource
    {
        public DocumentationSource(string basePath, ObjectDocument document, DocumentationFormat format = DocumentationFormat.MarkDown)
        {
            if (!Directory.Exists(basePath))
                throw new ArgumentException($"Basepath [ {basePath} ] does not exists", nameof(basePath));

            this.basePath = basePath;
            pathOffset = GetPathOffset(document);
            markdownPipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            Format = format;
        }

        public DocumentationFormat Format { get; }

        private static int GetPathOffset(ObjectDocument document)
        {
            var parts = document.Namespace.Split('.');
            return parts.Length;
        }

        public string[] GetPaths(string name)
        {
            var parts = name.Split('.');

            if (parts.Length < pathOffset + 1)
                throw new ArgumentException($"Invalid element name: {name}");

            if (parts.Length == pathOffset + 1)
                return new string[] { basePath };

            string[] retValue = new string[parts.Length - pathOffset];

            retValue[0] = basePath;

            for (int i = pathOffset; i < parts.Length - 1; ++i)
            {
                retValue[i - pathOffset + 1] = parts[i];
            }

            return retValue;
        }

        public string GetElementPath(string name)
        {
            var parts = GetPaths(name);

            if (parts.Length == 0)
                return basePath;

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
            new(Path: GetElementPath(name), Name: GetElementName(name), Format: Format);

        public string this[string? filename]
        {
            get
            {
                if (filename is null)
                    return string.Empty;

                if (File.Exists(filename))
                {
                    var text = File.ReadAllText(filename);

                    if (Format == DocumentationFormat.Text)
                    {
                        return text;
                    }

                    if (!string.IsNullOrEmpty(text))
                    {
                        return Markdown.ToHtml(text, markdownPipeline).Trim();
                    }

                    return string.Empty;
                }

                return string.Empty;
            }
        }

        private readonly string basePath;
        private readonly int pathOffset;
        private readonly MarkdownPipeline markdownPipeline;
    }
}
