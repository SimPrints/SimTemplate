using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateBuilder.ViewModel.Progress
{
    public class ProgressViewModel : ViewModel
    {
        private int m_Progress;

        public int Progress
        {
            get { return m_Progress; }
            set
            {
                if (m_Progress != value)
                {
                    m_Progress = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
