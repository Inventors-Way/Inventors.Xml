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

        public XSDGenerator(ObjectDocument document, DocumentationProvider documentation)
        {
            this.document = document;
            this.documentation = documentation;
        }

        public string FileName => $"{document.Root.Name}.xsd";        

        public string Run()
        {
            if (executed)
                return builder.ToString();

            if (document?.Root is null)
                throw new InvalidOperationException("No root type found in the document");

            executed = true;

            builder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            builder.AppendLine("<xs:schema elementFormDefault=\"qualified\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">");
            builder.AppendLine();
            builder.AppendLine($"<xs:element name=\"{document.Root.Name}\" nillable=\"true\" type=\"{document.Root.Type}\" />");

            document.Run(this);

            builder.AppendLine("</xs:schema>");
            return builder.ToString();
        }

        public void Visit(NullElement element) { }

        public void Visit(TypeElement element)
        {
            var orderOperator = element.Elements.Any(e => e.Choice) ? "sequence" : "all";

            builder.AppendLine();
            builder.AppendLine($"<xs:complexType name=\"{element.Name}\">");
            Annotate(GetDocumentation(element.Documentation));

            if (element.Elements.Count > 0)
            {
                builder.AppendLine($"<xs:{orderOperator}>");

                foreach (var e in element.Elements)
                {
                    if (e.Choice)
                    {
                        if (document[e.Type] is not ChoiceElement choice)
                            throw new InvalidOperationException("Element is not a choice element");

                        if (choice.Multiple)
                        {
                            builder.AppendLine($"<xs:choice minOccurs=\"0\" maxOccurs=\"unbounded\">");
                        }
                        else
                        {
                            builder.AppendLine($"<xs:choice minOccurs=\"1\" maxOccurs=\"1\">");
                        }

                        foreach (var item in choice.Choices)
                        {
                            builder.AppendLine($"<xs:element minOccurs=\"0\" maxOccurs=\"1\" name=\"{item.Name}\" type=\"{item.Type}\" />");
                        }

                        builder.AppendLine($"</xs:choice>");

                    }
                    else
                    {
                        if (string.IsNullOrEmpty(e.Documentation))
                        {
                            builder.AppendLine($"<xs:element minOccurs=\"{MinOccurs(e)}\" maxOccurs=\"1\" name=\"{e.Name}\" type=\"{e.Type}\" />");
                        }
                        else
                        {
                            builder.AppendLine($"<xs:element minOccurs=\"{MinOccurs(e)}\" maxOccurs=\"1\" name=\"{e.Name}\" type=\"{e.Type}\">");
                            Annotate(GetDocumentation(e.Documentation));
                            builder.AppendLine("</xs:element>");
                        }
                    }
                }

                builder.AppendLine($"</xs:{orderOperator}>");
            }

            if (element.Attributes.Count > 0)
            {
                foreach (var a in element.Attributes)
                {
                    if (string.IsNullOrEmpty(a.Documentation))
                    {
                        builder.AppendLine($"<xs:attribute name=\"{a.Name}\" type=\"{AttributeType(a)}\" use=\"{Required(a)}\" />");
                    }
                    else
                    {
                        builder.AppendLine($"<xs:attribute name=\"{a.Name}\" type=\"{AttributeType(a)}\" use=\"{Required(a)}\">");
                        Annotate(GetDocumentation(a.Documentation));
                        builder.AppendLine("</xs:attribute>");
                    }
                }
            }

            builder.AppendLine("</xs:complexType>");
        }

        public void Visit(ArrayElement element)
        {
            builder.AppendLine();
            builder.AppendLine($"<xs:complexType name=\"{element.Name}\">");

            builder.AppendLine($"<xs:choice minOccurs=\"0\" maxOccurs=\"unbounded\">");

            foreach (var item in element.Items)
            {
                builder.AppendLine($"<xs:element minOccurs=\"1\" maxOccurs=\"1\" name=\"{item.Name}\" nillable=\"true\" type=\"{item.Type}\" />");
            }

            builder.AppendLine($"</xs:choice>");
            builder.AppendLine($"</xs:complexType>");
        }

        public void Visit(ChoiceElement element)
        {
        }

        public void Visit(EnumElement element)
        {
            builder.AppendLine();
            builder.AppendLine($"<xs:simpleType name=\"{element.Name}\">");
            Annotate(GetDocumentation(element.Documentation));
            builder.AppendLine($"<xs:restriction base=\"xs:string\">");


            foreach (var value in element.Values)
            {
                if (string.IsNullOrEmpty(value.Documentation))
                {
                    builder.AppendLine($"<xs:enumeration value=\"{value.Name}\" />");
                }
                else
                {
                    builder.AppendLine($"<xs:enumeration value=\"{value.Name}\">");
                    Annotate(GetDocumentation(value.Documentation));
                    builder.AppendLine($"</xs:enumeration>");
                }
            }

            builder.AppendLine($"</xs:restriction>");
            builder.AppendLine("</xs:simpleType>");
        }


        private static string MinOccurs(ElementDescriptor d) => d.Required ? "1" : "0";

        private static string Required(AttributeDescriptor a) => a.Required ? "required" : "optional";

        private static string AttributeType(AttributeDescriptor a) => a.Primitive ? $"xs:{a.Type}" : a.Type;

        public string GetDocumentation(string id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;

            if (documentation is null)
                return string.Empty;

            if (id.StartsWith("@"))
                return documentation[id.Substring(1)];
            else
                return id;
        }

        public void Annotate(string? content)
        {
            if (string.IsNullOrEmpty(content))
                return;

            builder.AppendLine("<xs:annotation>");
            builder.AppendLine("<xs:documentation>");
            builder.AppendLine(content);
            builder.AppendLine("</xs:documentation>");
            builder.AppendLine("</xs:annotation>");
        }

        private readonly StringBuilder builder = new();
        private readonly ObjectDocument document;
        private readonly DocumentationProvider? documentation;
        private bool executed = false;
    }
}
