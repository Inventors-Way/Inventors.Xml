using CommandLine;
using System;

namespace xsdg 
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<SchemaOptions, DocumentationOptions>(args)
                .WithParsed<SchemaOptions>(options => options.Run())
                .WithParsed<DocumentationOptions>(options => options.Run())
                .WithNotParsed(errors =>
                {
                    foreach (var error in errors) 
                    {
                        Console.WriteLine($"{error}");
                    }
                });
        }
    }
}