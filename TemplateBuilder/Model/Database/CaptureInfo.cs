using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SimTemplate.Model.Database
{
    public class CaptureInfo
    {
        private readonly string m_HumanId;
        private readonly long m_DbId;
        private readonly byte[] m_ImageData;
        private readonly byte[] m_TemplateData;

        public string HumanId { get { return m_HumanId; } }
        public long DbId { get { return m_DbId; } }
        public byte[] ImageData { get { return m_ImageData; } }
        public byte[] TemplateData { get { return m_TemplateData; } }

        public CaptureInfo(string humanId, long dbId, byte[] imageData, byte[] templateData)
        {
            m_HumanId = humanId;
            m_DbId = dbId;
            m_ImageData = imageData;
            m_TemplateData = templateData;
        }
    }
}
