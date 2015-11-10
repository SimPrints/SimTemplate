using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateBuilderMVVM.Types
{
    public class Minutia
    {
        private readonly MinutiaRecord m_MinutiaRecord;
        private readonly MinutiaShapes m_MinutiaShapes;

        public MinutiaRecord Record { get { return m_MinutiaRecord; } }
        public MinutiaShapes Shapes { get { return m_MinutiaShapes; } }

        public Minutia(MinutiaRecord record, MinutiaShapes shapes)
        {
            m_MinutiaRecord = record;
            m_MinutiaShapes = shapes;
        }
    }
}
