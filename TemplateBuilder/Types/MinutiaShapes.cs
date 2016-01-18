using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace TemplateBuilderMVVM.Types
{
    public class MinutiaShapes
    {
        private Ellipse m_Location;
        private Line m_Direction;

        public Ellipse Location { get {return m_Location; } }
        public Line Direction { get {return m_Direction; } }

        public MinutiaShapes(Ellipse location, Line direction)
        {
            m_Location = location;
            m_Direction = direction;
        }
    }
}
