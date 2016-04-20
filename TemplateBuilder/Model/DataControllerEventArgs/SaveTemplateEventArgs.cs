using SimTemplate.Model.DataControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.Model.DataControllerEventArgs
{
    public class SaveTemplateEventArgs : EventArgs
    {
        private DataRequestResult m_Result;
        private Guid m_RequestId;

        public DataRequestResult Result { get { return m_Result; } }
        public Guid RequestId { get { return m_RequestId; } }

        public SaveTemplateEventArgs(Guid requestId, DataRequestResult result)
        {
            m_RequestId = requestId;
            m_Result = result;
        }
    }
}
