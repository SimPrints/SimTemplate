using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.Model.DataControllers.EventArguments
{
    public class InitialisationCompleteEventArgs : EventArgs
    {
        private readonly InitialisationResult m_Result;
        private readonly Guid m_RequestId;
        private readonly DataRequestResult m_RequestResult;

        public InitialisationResult Result { get { return m_Result; } }
        public Guid RequestId { get { return m_RequestId; } }

        public InitialisationCompleteEventArgs(InitialisationResult result, Guid requestId, DataRequestResult requestResult)
        {
            m_Result = result;
            m_RequestId = requestId;
            m_RequestResult = requestResult;
        }
    }
}
