
using System.Reflection;

namespace Inventors.Xml.Configuration
{
    public interface IJobConfiguration
    {
        string DocumentationPath { get; } 

        string OutputPath { get; } 

        string InputPath { get; }

        Assembly? Assembly { get; }
    }
}
