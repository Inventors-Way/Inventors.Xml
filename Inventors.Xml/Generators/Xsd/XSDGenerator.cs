﻿using Inventors.Xml.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Generators.Xsd
{
    public class XSDGenerator :
        IElementVisitor
    {
        public XSDGenerator(ObjectDocument document)
        {
            this.document = document;
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

        private string MinOccurs(ElementDescriptor d) => d.Required ? "1" : "0";

        private string Required(AttributeDescriptor a) => a.Required ? "required" : "optional";

        private string AttributeType(AttributeDescriptor a) => a.Primitive ? $"xs:{a.Type}" : a.Type;

        public void CreateNonderivedType(ClassElement element)
        {
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
                        builder.AppendLine($"<xs:element minOccurs=\"{MinOccurs(e)}\" maxOccurs=\"1\" name=\"{e.Name}\" type=\"{e.Type.Name}\" />");
                    }
                }

                builder.AppendLine("</xs:sequence>");
            }

            if (element.Attributes.Count > 0)
            {
                foreach (var a in element.Attributes)
                {
                    builder.AppendLine($"<xs:attribute name=\"{a.Name}\" type=\"{AttributeType(a)}\" use=\"{Required(a)}\" />");
                }
            }
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
        private bool executed = false;
    }

}