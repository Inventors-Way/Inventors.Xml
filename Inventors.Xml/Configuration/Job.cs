﻿using Inventors.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Throw;

namespace Inventors.Xml.Configuration
{
    public abstract class Job
    {
        [XmlAttribute("title")]
        [XmlRequired(true)]
        [XmlDocumentation("@Configuration.Job.Title.md")]
        public string Title { get; set; } = string.Empty;

        [XmlAttribute("type")]
        [XmlRequired(true)]
        [XmlDocumentation("@Configuration.Job.Type.md")]
        public string Type { get; set; } = string.Empty;

        protected Type LoadType(IJobConfiguration configuration)
        {
            if (configuration.Assembly is not Assembly assembly)
                throw new InvalidOperationException("Assembly not loaded");

            if (assembly.GetType(Type) is not Type type)
                throw new InvalidOperationException($"Did not get type [ {Type} ]");

            return type;
        }
            

        protected static string GetOutputPath(string path, IJobConfiguration c) =>
            string.IsNullOrEmpty(c.OutputPath) ? path : Path.Combine(new string[]
                {
                    path,
                    c.OutputPath
                });

        public abstract void Run(string path, IJobConfiguration configuration, IDocumentationSource? docSource = null, bool verbose = false);
    }
}
