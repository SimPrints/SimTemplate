using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SimTemplate.DataTypes
{
    public class CaptureInfo
    {
        private readonly long m_DbId;
        private readonly byte[] m_ImageData;
        private readonly byte[] m_TemplateData;

        public long DbId { get { return m_DbId; } }
        public byte[] ImageData { get { return m_ImageData; } }
        public byte[] TemplateData { get { return m_TemplateData; } }

        public CaptureInfo(long dbId, byte[] imageData, byte[] templateData)
        {
            m_DbId = dbId;
            m_ImageData = imageData;
            m_TemplateData = templateData;
        }
    }
}
