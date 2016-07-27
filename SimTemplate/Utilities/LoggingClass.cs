using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.Utilities
{
    public abstract class LoggingClass
    {
        private ILog m_Log;

        protected ILog Log
        {
            get { return LogManager.GetLogger(this.GetType()); }
        }
    }
}
