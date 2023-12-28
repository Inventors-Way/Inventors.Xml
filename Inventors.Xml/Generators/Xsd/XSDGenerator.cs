using Inventors.Xml.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Inventors.Xml.Generators.Xsd
{
    public class XSDGenerator :
        IElementVisitor
    {
        public XSDGenerator(ObjectDocument document)
        {
            this.document = document;
            documentation = null;
        }

        public XSDGenerator(ObjectDocument document, DocumentationSource documentation)
        {
            this.document = document;
            this.documentation = documentation;
        }

        public string Run()
        {
            if (executed)
                return builder.ToString();

            executed = true;

            builder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            builder.AppendLine("<xs:schema elementFormDefault=\"qualified\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">");
            builder.AppendLine($"<xs:element name=\"{document.Root.Name}\" nillable=\"true\" type=\"{document.Root.Type.Name}\" />");

            document.Run(this);

            builder.AppendLine("</xs:schema>");
            return builder.ToString();
        }

        public void Visit(NullElement element) { }

        public void Visit(ArrayElement element)
        {
            builder.AppendLine();
            builder.AppendLine($"<xs:complexType name=\"{element.Name}\">");
            builder.AppendLine($"<xs:choice minOccurs=\"0\" maxOccurs=\"unbounded\">");

            foreach (var item in element.Items)
            {
                builder.AppendLine($"<xs:element minOccurs=\"1\" maxOccurs=\"1\" name=\"{item.Name}\" nillable=\"true\" type=\"{item.Type.Name}\" />");
            }

            builder.AppendLine($"</xs:choice>");
            builder.AppendLine($"</xs:complexType>");
        }

        public void Visit(ChoiceElement element)
        {
            if (element.Multiple)
            {
                builder.AppendLine($"<xs:choice minOccurs=\"0\" maxOccurs=\"unbounded\">");
            }
            else
            {
                builder.AppendLine($"<xs:choice minOccurs=\"1\" maxOccurs=\"1\">");
            }

            foreach (var choice in element.Choices)
            {
                builder.AppendLine($"<xs:element minOccurs=\"0\" maxOccurs=\"1\" name=\"{choice.Name}\" type=\"{choice.Type.Name}\" />");
            }

            builder.AppendLine("</xs:choice>");
        }

        public void Visit(ClassElement element)
        {
            builder.AppendLine();

            if (element.IsAbstract)
            {
                builder.AppendLine($"<xs:complexType name=\"{element.Name}\" abstract=\"true\">");
            }
            else
            {
                builder.AppendLine($"<xs:complexType name=\"{element.Name}\">");
            }

            if (element.IsDerived)
            {
                CreateDerivedType(element);
            }
            else
            {
                CreateNonderivedType(element);
            }

            builder.AppendLine("</xs:complexType>");
        }

        private void CreateDerivedType(ClassElement element)
        {
            builder.AppendLine("<xs:complexContent mixed=\"false\">");
            builder.AppendLine($"<xs:extension base=\"{element.BaseType}\">");

            CreateNonderivedType(element);

            builder.AppendLine("</xs:extension>");
            builder.AppendLine("</xs:complexContent>");
        }

        private static string MinOccurs(ElementDescriptor d) => d.Required ? "1" : "0";

        private static string Required(AttributeDescriptor a) => a.Required ? "required" : "optional";

        private static string AttributeType(AttributeDescriptor a) => a.Primitive ? $"xs:{a.Type}" : a.Type;

        public void CreateNonderivedType(ClassElement element)
        {
            var info = AnnotateElement(element);
            
            if (element.Elements.Count > 0)
            {
                builder.AppendLine("<xs:sequence>");

                foreach (var e in element.Elements)
                {
                    if (e.Type.IsNested)
                    {
                        e.Type.Accept(this);
                    }
                    else
                    {
                        var annotation = GetDocumentation(info, e.PropertyName);

                        if (annotation is null)
                        {
                            builder.AppendLine($"<xs:element minOccurs=\"{MinOccurs(e)}\" maxOccurs=\"1\" name=\"{e.Name}\" type=\"{e.Type.Name}\" />");
                        }
                        else
                        {
                            builder.AppendLine($"<xs:element minOccurs=\"{MinOccurs(e)}\" maxOccurs=\"1\" name=\"{e.Name}\" type=\"{e.Type.Name}\">");
                            Annotate(annotation);
                            builder.AppendLine("</xs:element>");
                        }
                    }
                }

                builder.AppendLine("</xs:sequence>");
            }

            if (element.Attributes.Count > 0)
            {
                foreach (var a in element.Attributes)
                {
                    var annotation = GetDocumentation(info, a.PropertyName);

                    if (annotation is null)
                    {
                        builder.AppendLine($"<xs:attribute name=\"{a.Name}\" type=\"{AttributeType(a)}\" use=\"{Required(a)}\" />");
                    }
                    else
                    {
                        builder.AppendLine($"<xs:attribute name=\"{a.Name}\" type=\"{AttributeType(a)}\" use=\"{Required(a)}\">");
                        Annotate(annotation);
                        builder.AppendLine("</xs:attribute>");
                    }
                }
            }
        }

        public string? GetDocumentation(ElementDocumentationInfo? info, string propertyName)
        {
            if (info is null)
                return null;

            if (documentation is null)
                return null;

            var content = documentation[info.GetFilename(propertyName)];

            if (string.IsNullOrEmpty(content))
                return null;

            return content;
        }

        public ElementDocumentationInfo? AnnotateElement(Element element)
        {
            if (documentation is null)
                return null;

            var info = documentation.GetElement(element.Name);
            var content = documentation[info.GetFilename()];

            Annotate(content);

            return info;
        }

        public void Annotate(string content)
        {
            if (string.IsNullOrEmpty(content))
                return;

            builder.AppendLine("<xs:annotation>");
            builder.AppendLine("<xs:documentation>");
            builder.AppendLine(content);
            builder.AppendLine("</xs:documentation>");
            builder.AppendLine("</xs:annotation>");
        }

        public void Visit(EnumElement element)
        {
            builder.AppendLine();
            builder.AppendLine($"<xs:simpleType name=\"{element.Name}\">");
            builder.AppendLine($"<xs:restriction base=\"xs:string\">");

            foreach (var value in element.Values)
            {
                builder.AppendLine($"<xs:enumeration value=\"{value}\" />");
            }

            builder.AppendLine($"</xs:restriction>");
            builder.AppendLine("</xs:simpleType>");
        }

        private readonly StringBuilder builder = new();
        private readonly ObjectDocument document;
        private readonly DocumentationSource? documentation;
        private bool executed = false;
    }

}
