using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimTemplate.Utilities;
using SimTemplate.DataTypes;

namespace SimTemplate.Model.DataControllers.EventArguments
{
    public class GetCaptureCompleteEventArgs : EventArgs
    {
        private CaptureInfo m_Capture;
        private Guid m_RequestId;
        private DataRequestResult m_Result;

        public CaptureInfo Capture { get { return m_Capture; } }

        public Guid RequestId { get { return m_RequestId; } }

        public DataRequestResult Result { get { return m_Result; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetCaptureCompleteEventArgs"/> class in 
        /// the case where the request was successful.
        /// </summary>
        /// <param name="capture">The capture.</param>
        /// <param name="requestId">The request unique identifier.</param>
        public GetCaptureCompleteEventArgs(CaptureInfo capture, Guid requestId, DataRequestResult result)
        {
            IntegrityCheck.IsNotNull(requestId);
            IntegrityCheck.IsNotNull(result);
            if (result == DataRequestResult.Success)
            {
                IntegrityCheck.IsNotNull(capture);
            }

            m_Capture = capture;
            m_RequestId = requestId;
            m_Result = result;
        }
    }
}
