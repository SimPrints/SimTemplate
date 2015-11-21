using log4net;
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
using TemplateBuilder.Model;

namespace TemplateBuilder.ViewModel.States
{
    abstract public class State
    {
        private ILog m_Log;

        protected TemplateBuilderViewModel m_Outer;
        protected StateManager m_StateMgr;

        public State(TemplateBuilderViewModel outer, StateManager stateMgr)
        {
            m_Outer = outer;
            m_StateMgr = stateMgr;
        }

        // TODO: move this off State and onto 'Initialised'? abstract state.

        virtual public void OnEnteringState() { }

        virtual public void OnLeavingState() { }

        abstract public void OpenFile();

        abstract public void OpenFolder();

        abstract public void PositionInput(Point point);

        abstract public void PositionMove(Point point);

        abstract public void RemoveItem(int index);

        abstract public void SaveTemplate();

        abstract public void image_SizeChanged(Size newSize);

        virtual public bool IsMinutiaTypeButtonsEnabled { get { return false; } }

        public string Name { get { return GetType().Name; } }

        protected ILog Logger
        {
            get
            {
                if (m_Log == null)
                {
                    m_Log = LogManager.GetLogger(this.GetType());
                }
                return m_Log;
            }
        }
    }
}
