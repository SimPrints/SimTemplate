// Copyright 2016 Sam Briggs
//
// This file is part of SimTemplate.
//
// SimTemplate is free software: you can redistribute it and/or modify it under the
// terms of the GNU General Public License as published by the Free Software 
// Foundation, either version 3 of the License, or (at your option) any later
// version.
//
// SimTemplate is distributed in the hope that it will be useful, but WITHOUT ANY 
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
// A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// SimTemplate. If not, see http://www.gnu.org/licenses/.
//
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

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the position of the minutia on the image.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
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

        /// <summary>
        /// Gets or sets the type of the minutia.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public MinutiaType Type
        {
            get { return m_Type; }
            set
            {
                m_Type = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MinutiaRecord"/> class.
        /// </summary>
        public MinutiaRecord() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MinutiaRecord"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="type">The type.</param>
        public MinutiaRecord(Point position, double angle, MinutiaType type)
        {
            m_Position = position;
            m_Angle = angle;
            m_Type = type;
        }

        #region Equals

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            MinutiaRecord p = obj as MinutiaRecord;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return Equals(p);
        }

        public bool Equals(MinutiaRecord record)
        {
            // First check if positions are equal (least likely)
            bool isEqual = record.Position == m_Position;
            if (isEqual)
            {
                // Next check if angles are equal
                isEqual = (record.Angle == m_Angle);
                if (isEqual)
                {
                    // Finally test if the types are the same
                    isEqual = record.Type == m_Type;
                }
            }
            return isEqual;
        }
        public override int GetHashCode()
        {
            return (int)m_Position.X ^
                (int)m_Position.Y ^
                (int)m_Angle ^
                (int)m_Type;
        }

        #endregion

        #region INotifyPropertyChanged

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

        #endregion
    }
}
