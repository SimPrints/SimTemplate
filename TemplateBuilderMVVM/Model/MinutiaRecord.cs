using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TemplateBuilderMVVM.Model
{
    public class MinutiaRecord : INotifyPropertyChanged
    {
        private Point m_Location;
        private double m_Direction;
        private MinutiaType m_Type;

        public event PropertyChangedEventHandler PropertyChanged;

        public Point Location
        {
            get { return m_Location; }
            set
            {
                m_Location = value;
                NotifyPropertyChanged();
            }
        }
        public double Direction
        {
            get { return m_Direction; }
            set
            {
                m_Direction = value;
                NotifyPropertyChanged();
            }
        }
        public MinutiaType Type
        {
            get { return m_Type; }
            set
            {
                m_Type = value;
                NotifyPropertyChanged();
            }
        }
        public MinutiaRecord() { }

        public MinutiaRecord(Point location, double direction, MinutiaType type)
        {
            m_Location = location;
            m_Direction = direction;
            m_Type = type;
        }

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
