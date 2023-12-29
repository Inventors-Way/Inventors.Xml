using CommandLine;
using System;

namespace xsdg 
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => options.Run())
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