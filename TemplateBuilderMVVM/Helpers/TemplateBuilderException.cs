using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateBuilder.Helpers
{
    public class TemplateBuilderException : Exception
    {
        public TemplateBuilderException() : base() { }
        public TemplateBuilderException(string message) : base(message) { }
        public TemplateBuilderException(string message, Exception innerException) : base(message, innerException) { }
    }
}
