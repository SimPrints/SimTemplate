using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateBuilder.Model.Database
{
    public class GetCaptureCompleteEventArgs : EventArgs
    {
        private CaptureInfo m_Capture;
        private Guid m_RequestGuid;

        public CaptureInfo Capture { get { return m_Capture; } }

        public Guid RequestGuid { get { return m_RequestGuid; } }

        public GetCaptureCompleteEventArgs(CaptureInfo capture, Guid requestGuid)
        {
            m_Capture = capture;
            m_RequestGuid = requestGuid;
        }
    }
}
