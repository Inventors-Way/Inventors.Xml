using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml.Configuration
{
    public static class JobExtensions
    {
        public static T Run<T>(this string str, Func<T> func)
        {
            Console.Write($"{str} ... ");
            T retValue = func();
            Console.WriteLine("done");
            return retValue;
        }
    }
}
