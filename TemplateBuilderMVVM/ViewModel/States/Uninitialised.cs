using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TemplateBuilderMVVM.Helpers;
using TemplateBuilderMVVM.Model;

namespace TemplateBuilderMVVM.ViewModel.States
{
    public class Uninitialised : State
    {
        public Uninitialised(TemplateBuilderViewModel outer, StateManager stateMgr) : base(outer, stateMgr) { }

        public override void OnEnteringState()
        {
            base.OnEnteringState();

            // Initialise properties.
            m_Outer.Minutae = new TrulyObservableCollection<MinutiaRecord>();
            m_Outer.InputMinutiaType = MinutiaType.Termination;

            // Transition to Initialised.
            m_StateMgr.TransitionTo(typeof(Idle));
        }

        public override void image_SizeChanged(Size newSize)
        {
            // Do nothing. Uninitialised.
        }

        public override void OpenFile()
        {
            // Do nothing. Uninitialised.
        }

        public override void OpenFolder()
        {
            // Do nothing. Uninitialised.
        }

        public override void PositionInput(Point e)
        {
            // Do nothing. Uninitialised.
        }

        public override void PositionMove(Point e)
        {
            // Do nothing. Uninitialised.
        }

        public override void RemoveItem(int index)
        {
            // Do nothing. Uninitialised.
        }

        public override void SaveTemplate()
        {
            // Do nothing. Uninitialised.
        }
    }
}
