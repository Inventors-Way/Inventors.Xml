using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Inventors.Xml.Serialization;
using Inventors.Xml.Test.TestObjects.Data;

namespace Inventors.Xml.Test.TestObjects
{
    [XmlRoot("company")]
    public class Company
    {
        
        [XmlAttribute("name")]
        [XmlRequired(true)]
        public string Name { get; set; } = string.Empty;

        [XmlArray("departments")]
        [XmlArrayItem("department")]
        public List<Department> Departments { get; } = new List<Department>();

        [XmlArray("employees")]
        [XmlArrayItem("employee", typeof(Employee))]
        public List<Employee> Employees { get; } = new List<Employee>();

        [XmlArray("projects")]
        [XmlArrayItem("project", typeof(Project))]
        public List<Project> Projects { get; } = new List<Project>();

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"COMPANY [ {Name} ]");

            if (Departments.Count > 0)
            {
                builder.AppendLine("DEPARTMENTS:");

                foreach (var department in Departments)
                {
                    builder.AppendLine(department.ToString());
                }
            }

            if (Employees.Count > 0) 
            {
                builder.AppendLine("EMPLOYEES:");

                foreach (var employee in Employees)
                {
                    builder.AppendLine($"- {employee}");
                }
            }

            if (Projects.Count > 0) 
            {
                builder.AppendLine("PROJECTS:");

                foreach (var project in Projects)
                {
                    builder.AppendLine(project.ToString());
                }
            }

            return builder.ToString();
        }
    }
}
