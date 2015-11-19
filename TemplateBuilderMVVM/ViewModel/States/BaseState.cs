using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TemplateBuilderMVVM.ViewModel.States
{
    abstract public class BaseState : State
    {
        public BaseState(TemplateBuilderViewModel outer, StateManager stateMgr) : base(outer, stateMgr)
        { }

        public override void LoadFile()
        {
            OpenFileDialog od = new OpenFileDialog();
            //od.Filter = "XLS files|*.xls";
            //od.Multiselect = true;
            if (od.ShowDialog() == true)
            {
                // TODO: only pass valid file types.

                // Record image and filename
                m_Outer.ImageFileName = od.FileName;
                m_Outer.Image = new BitmapImage(new Uri(od.FileName));

                // Transition state to 'Templating'
                m_StateMgr.TransitionTo(typeof(WaitLocation));
            }
        }
    }
}
