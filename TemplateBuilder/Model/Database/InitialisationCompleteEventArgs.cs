using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateBuilder.Model.Database
{
    public class InitialisationCompleteEventArgs : EventArgs
    {
        private readonly bool m_IsSuccessful;

        public bool IsSuccessful { get { return m_IsSuccessful; } }

        public InitialisationCompleteEventArgs(bool isSuccessful)
        {
            m_IsSuccessful = isSuccessful;
        }
    }
}
