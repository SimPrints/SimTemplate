using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TemplateBuilderMVVM.ViewModel.States
{
    abstract public class State
    {
        protected TemplateBuilderViewModel m_Outer;
        protected StateManager m_StateMgr;

        public State(TemplateBuilderViewModel outer, StateManager stateMgr)
        {
            m_Outer = outer;
            m_StateMgr = stateMgr;
        }

        public void LoadFile()
        {
            OpenFileDialog od = new OpenFileDialog();
            //od.Filter = "XLS files|*.xls";
            //od.Multiselect = true;
            if (od.ShowDialog() == true)
            {
                // TODO: only pass valid file types.

                // Record image filename
                m_Outer.ImageFile = od.FileName;

                // Transition state to 'Templating'
                m_StateMgr.TransitionTo(typeof(WaitLocation));
            }
        }

        virtual public void OnEnteringState() { }

        virtual public void OnLeavingState() { }

        abstract public void PositionInput(Point e);

        abstract public void PositionMove(Point e);

        abstract public void RemoveItem(int index);

        abstract public void SaveTemplate();

        public string Name { get { return GetType().Name; } }
    }
}
