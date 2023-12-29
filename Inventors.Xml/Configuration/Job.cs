﻿using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.Xml.Configuration
{
    public abstract class Job
    {
        [XmlAttribute("title")]
        [XmlRequired(true)]
        public string Title { get; set; } = string.Empty;

        [XmlAttribute("type")]
        [XmlRequired(true)]
        public string Type { get; set; } = string.Empty;

        protected Type LoadType(string path, IJobConfiguration configuration)
        {
            var assemblyPath = Path.Combine(path, configuration.Assembly);

            if (!File.Exists(assemblyPath))
            {
                Console.WriteLine($"Did not find assembly: {assemblyPath}");
                throw new ArgumentException("Assembly not found");
            }

            var assembly = Assembly.LoadFrom(assemblyPath);
            var type = assembly.GetType(Type);

            return type ?? throw new InvalidOperationException($"Failed to load type [ {Type} ]");
        }

        public abstract void Run(string path, IJobConfiguration configuration);
    }
}
