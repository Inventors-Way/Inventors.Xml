# Generation of Documentation Files templates

The xsdg tool can include documentation from Text, Markdown, or HTML files. Each the ```xs:documention``` that will be included for classes and properties comes from one file for each entity. The documentation file for a class has the same name as the class and must be located in a folder structure matching the namespaces of the class. Documentation files for each property within the class have the name of [ClassName].[PropertyNAme] and must be located next to the class documentation file.

This job will create a full set of documentation files for all classes and properties in the class hirarchy of a class. All files be empty and have the correct extension for the selected input format.

Please note that this job will never overwrite files. If a documentation file for a class or property allready exists then this documentation job will not generate a documentation file for this entity. Consequently, there is no risk in running this job. Furthermore, this job will also never delete a documentation file. If a class or property has been deleted then any documentation file for this deleted entity will be left.