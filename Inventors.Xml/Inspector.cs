using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml
{
    public class Inspector
    {
        public static ObjectDocument Run(Type input) => ObjectDocument.Create(input);
    }
}
