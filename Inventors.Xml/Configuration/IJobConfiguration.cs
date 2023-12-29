
namespace Inventors.Xml.Configuration
{
    public interface IJobConfiguration
    {
        string Assembly { get; } 
        string DocumentationPath { get; } 
        string OutputPath { get; } 
        string InputPath { get; } 
    }
}
