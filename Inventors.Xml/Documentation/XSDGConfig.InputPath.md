# Input path for the Assembly

This attribute specifies the relative path to the Assembly which contains the types to be analysed by the xsdg tool. This attribute is optional. If it is not specified then the Assembly will be presumed located in the current working directory.

Please note, this path must not start with a path separator, as this will cause the working directory path to be discarded; most likely cause an error when running the program.