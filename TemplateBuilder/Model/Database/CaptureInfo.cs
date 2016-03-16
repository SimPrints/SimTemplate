using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TemplateBuilder.Model.Database
{
    public class CaptureInfo
    {
        private readonly string m_Guid;
        private readonly long m_DbId;
        private readonly byte[] m_ImageData;
        private readonly byte[] m_TemplateData;

        public string Guid { get { return m_Guid; } }
        public long DbId { get { return m_DbId; } }
        public byte[] ImageData { get { return m_ImageData; } }
        public byte[] TemplateData { get { return m_TemplateData; } }

        public CaptureInfo(string guid, long dbId, byte[] imageData, byte[] templateData)
        {
            m_Guid = guid;
            m_DbId = dbId;
            m_ImageData = imageData;
            m_TemplateData = templateData;
        }
    }
}
