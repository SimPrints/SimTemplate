using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using SimTemplate.Utilities;
using SimTemplate.DataTypes.Enums;

namespace SimTemplate.DataTypes
{
    public class MinutiaRecord : INotifyPropertyChanged
    {
        private Point m_Position;
        private double m_Angle;
        private MinutiaType m_Type;

        public event PropertyChangedEventHandler PropertyChanged;

        public Point Position
        {
            get { return m_Position; }
            set
            {
                m_Position = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the angle of the minutia TODO: From horizontal/vertical?.
        /// NOTE: Must be positive.
        /// </summary>
        /// <value>
        /// The angle in degrees.
        /// </value>
        public double Angle
        {
            get { return m_Angle; }
            set
            {
                IntegrityCheck.IsTrue(value >= 0, "Minutia angle must be positive.");
                IntegrityCheck.IsTrue(value <= 360, "Minutia angle must be degree between 0 - 360.");
                m_Angle = value;
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

        public MinutiaRecord(Point position, double angle, MinutiaType type)
        {
            m_Position = position;
            m_Angle = angle;
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
