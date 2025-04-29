
using System.Reflection;

namespace Inventors.Xml.Configuration
{
    public interface IJobConfiguration
    {
        string OutputPath { get; } 

        string InputPath { get; }

        Assembly? Assembly { get; }
    }
}
