using SimTemplate.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.ViewModels.Interfaces
{
    public class ActivityChangedEventArgs : EventArgs
    {
        private readonly Activity m_NewActivity;

        public Activity NewActivity { get { return m_NewActivity; } }

        public ActivityChangedEventArgs(Activity newActivity)
        {
            m_NewActivity = newActivity;
        }
    }
}
