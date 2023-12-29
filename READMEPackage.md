# Inventors.Xml

Inventors.Xml combines an optional library  (Inventors.Xml.Serialization) and a tool (xsdg.exe) to replace the venerable xsd.exe tool from Microsoft. While this tool is handy for automatically generating XSD schemas from C# classes, it suffers from several drawbacks:

1. It provides no feature for controlling whether an attribute or element is required,
2. It provides no feature for including doccumentation elements within the generated XSD Schema, and
3. It can only be used on the .NET Framework platform; no porting of the tool to .NET platform appears in the work.

These three drawbacks inspired us to write the Inventors.Xml.Serialization library and xsdg.exe tool. The tool is written to document the [LabBench Language](https://labbench.io/) we are developing for describing neuroscience experiments, but its usefulness is not restricted to this particular XML-based language. Consequently, we have released the tool and library as open source (MIT), hoping others may benefit from our work. 

## Features

* Generation of XSD schemas from C# classes.
* Control of ```use``` for attributes and ```minOccurs``` for elements with an optional ```XmlRequiredAttribute``` from the Intentors.Xml.Serialization library.
* Useful extension methods for working with XML in the Inventors.Xml.Serialization library
* Generation of documentation annotations in the XSD schema from Text, Markdown, and HTML documentation files.
* Generation of a complete set of documentation files for a C# class hierarchy. 

## Usage

### xsdg.exe

The xsdg.exe can generate an XSD Schema for a C# class or a set of documentation file templates (please see the Documentation Files sectoin below). The general format for invoking the xsdg tool is:

```
xsdg -p [Working Directory] [Configuration File] 
```

where -p [Working Directory] is optional; if omitted the directory from which the command is invoked is used as the working directory for the tool. The configuration file describes the jobs that the tool should perform. Below is an example that first generates a set of documentation file templates and secondly generates a XSD schema:

```
<?xml version="1.0" encoding="utf-8" ?>
<xsdg 
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
    xsi:noNamespaceSchemaLocation="Schema/xsdg.xsd"
    assembly="Inventors.Xml"
    input-path="Inventors.Xml/bin/Debug/net6.0/"
    documentation-path="Inventors.Xml/Documentation/"
    output-path="xsdg/Schema/">
    
    <documentation
        title="Creating Markdown Documentation"
        type="Inventors.Xml.XSDGConfig"
        documentation-file-format="markdown"/>

    <schema
        title="Generating XML Schema"
        type="Inventors.Xml.XSDGConfig"
        include-documentation="true"
        documentation-file-format="markdown"
        documentation-output-format="html"
        encode-data="true"
        encapsulate-character-data="false"/>
</xsdg>
```

Information on how to configure the tool can be found in the XSD schema (xsdg.xsd) for the XSDG tool. This XSD schema was generated with the tool itself. If a suitable editor such as Visual Studio Code with the Redhat XML extension is used then documentation will be provided by hover over and when code completion is invoked while writing configuration files.

### Inventors.Xml.Serialization

The System.Xml.Serialization namespace contains no Attribute that can control whether an attribute or element in the resulting XML schema is required—the Inventors.Xml.Serialization library provides a single attribute  (XmlRequiredAttribute) that can be used on properties to control whether the resulting attributes or elements will be marked as required in the generated XML Schema.

The use of this Attribute is optional, and the Inventors.Xml.Serialization library is not required for the xsdg.exe tool to be used on an assembly. If the XmlRequiredAttribute is not used, then xsdg will generate XSD Schemas with the same conventions as the xsd.exe tool.

Besides the XmlRequiredAttribute, the library also contains several extension functions that we find helpful when working with XML data.

Below is an example of how the XmlRequired attribute has been used to control XSD schema generation for the class for the ```<schema>``` element above:

```C#
public class SchemaJob :
    Job
{
    [XmlAttribute("include-documentation")]
    [XmlRequired(false)]
    public bool IncludeDocumentation { get; set; } = true;

    [XmlAttribute("documentation-file-format")]
    [XmlRequired(false)]
    public DocumentationFormat DocumentationFileFormat { get; set; } = DocumentationFormat.MarkDown;

    [XmlAttribute("documentation-output-format")]
    [XmlRequired(false)]
    public DocumentationFormat DocumentationOutputFormat { get; set; } = DocumentationFormat.Html;

    [XmlAttribute("encode-data")]
    [XmlRequired(false)]
    public bool EncodeData { get; set; } = true;

    [XmlAttribute("encapsulate-character-data")]
    [XmlRequired(false)]    
    public bool EncapsulateCharacterData { get; set; } = false;
}
```

Please note the ```title``` and ```type``` attributes comes from its base class ```Job```.

### Extension methods

When the Inventors.Xml.Serialization namespace is used, the following extension methods becomes available:

#### Deserializing XML

Any string can be deserialized with the ```ToObject<T>``` extenstion method:

```C#
var project = xmlString.ToObject<Project>();
```

#### Serializing objects

Any object can be serialized into a string with the ```ToObject<T>``` extension method:

```C#
var xmlString = project.ToXML();
```

## Installation

### Nuget Package

### XSDG Tool



## Documentation Files

The xsdg tool can include documentation from Text, Markdown, or HTML files. Each the ```xs:documention``` that will be included for classes and properties comes from one file for each entity. The documentation file for a class has the same name as the class and must be located in a folder structure matching the namespaces of the class. Documentation files for each property within the class have the name of [ClassName].[PropertyNAme] and must be located next to the class documentation file.