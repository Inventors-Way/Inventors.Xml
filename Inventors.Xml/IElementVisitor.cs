using Inventors.Xml.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.Xml
{
    public interface IElementVisitor
    {
        void Visit(ArrayElement element);

        void Visit(ChoiceElement element);

        void Visit(TypeElement element);

        void Visit(EnumElement element);

        void Visit(NullElement element);
    }
}
