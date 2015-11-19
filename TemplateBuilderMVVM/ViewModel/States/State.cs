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
using TemplateBuilderMVVM.Model;

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

        // TODO: move this off State and onto 'Initialised'? abstract state.

        virtual public void OnEnteringState() { }

        virtual public void OnLeavingState() { }

        abstract public void LoadFile();

        abstract public void PositionInput(Point point);

        abstract public void SetMinutiaType(MinutiaType type);

        abstract public void PositionMove(Point point);

        abstract public void RemoveItem(int index);

        abstract public void SaveTemplate();

        abstract public void image_SizeChanged(Size newSize);

        virtual public bool IsMinutiaTypeButtonsEnabled { get { return false; } }

        public string Name { get { return GetType().Name; } }
    }
}
