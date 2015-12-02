using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateBuilder.ViewModel;

namespace TemplateBuilder.Model.Database
{
    public class ProgressChangedEventArgs : EventArgs
    {
        private readonly int m_Progress;
        private readonly DialogAction m_Action;

        public int Progress { get { return m_Progress; } }
        public DialogAction Action { get { return m_Action; } }

        public ProgressChangedEventArgs(int progress, DialogAction action)
        {
            m_Progress = progress;
            m_Action = action;
        }
    }
}
