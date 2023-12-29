# XSDG Configuration File

The xsdg tool is controlled by a configuration file, which specifies:

1. Defines paths to the Assembly, Documentation Files, and the Output Path for XSD Schemas. All paths within the configuration are relative to the working directory of the XSDG tool. 
2. The name of the Assembly that contains the types to be analyzed.
3. A number of jobs to be performed. Two jobs can be configured:
	1. Documentation jobs: which will generate templates for Documentation Files.
	2. Schema jobs: which will generate XSD Schemas for selected types.

This configuration file is passed to the xsdg tool as a parameter:

```
xsdg -p [working directory] [name of configuration file]
```

The ```-p [working directory]``` is optional and if specified will set the working directory of the xsdg tool. If it is not specied the working directory will be the directory from where the xsdg tool is invoked. 
