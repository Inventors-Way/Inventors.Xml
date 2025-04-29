# Path to XSD documentation

This attribute specifies the relative path to the Documentation Files that will be included in ```xsd:documention``` elements in generated XSD Schemas, or where templates for the Documentation Files will be generated. This attribute is optional. If it is not specified then the Documentation Files will be presumed located in the current working directory.

Please note, this path must not start with a path separator, as this will cause the working directory path to be discarded; most likely cause an error when running the program.