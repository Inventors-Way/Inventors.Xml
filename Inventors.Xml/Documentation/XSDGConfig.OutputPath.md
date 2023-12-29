# Output path for generated XSD Schemas

This attribute specifies the relative path to which generated XSD Schemas will be written. This attribute is optional. If it is not specified then XSD Schemas will be written to the current working directory.

Please note, this path must not start with a path separator, as this will cause the working directory path to be discarded; most likely cause an error when running the program.