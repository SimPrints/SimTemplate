using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.Utilities
{
    public class SimTemplateException : Exception
    {
        public SimTemplateException() : base() { }
        public SimTemplateException(string message) : base(message) { }
        public SimTemplateException(string message, Exception innerException) : base(message, innerException) { }
    }
}
