using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Serialization
{
    [AttributeUsage(AttributeTargets.Property)]
    public class XmlOrderAttribute :
        Attribute
    {
        public XmlOrderAttribute(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }

    public static class XmlOrderAttributeExtensions
    {
        public static int GetAttributeOrder(this PropertyInfo self, OrderGenerator generator)
        {
            if (self.GetCustomAttribute<XmlOrderAttribute>() is not XmlOrderAttribute order)
                return generator.Next();

            return order.Value;

        }
    }

    public class OrderGenerator
    {
        public int Next() => order++;

        public void Reset() => order = 0;

        private int order = 0;
    }
}
