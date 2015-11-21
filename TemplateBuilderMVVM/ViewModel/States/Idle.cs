using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using TemplateBuilderMVVM.Helpers;

namespace TemplateBuilderMVVM.ViewModel.States
{
    public class Idle : Initialised
    {
        public Idle(TemplateBuilderViewModel outer, StateManager stateMgr) : base(outer, stateMgr)
        { }

        #region Overriden Abstract Methods

        public override void OnEnteringState()
        {
            base.OnEnteringState();

            // Deactivate UI controls.
            m_Outer.IsSaveTemplatePermitted = false;
            m_Outer.IsInputMinutiaTypePermitted = false;

            // Hide old image from UI, and remove other things.
            m_Outer.Image = null;
            m_Outer.Minutae.Clear();

            // Transition to LoadFile if there is a legit file queued.
            ChoiceLoadFile();
        }

        public override void PositionInput(Point point)
        {
            // Do nothing.
        }

        public override void PositionMove(Point point)
        {
            // Do nothing.
        }

        public override void RemoveItem(int index)
        {
            // There should be no minutia visible in this state.
            // TODO: Transition to Error?
            throw IntegrityCheck.Fail(
                "There should be no minutia visible in this state.");
        }

        public override void SaveTemplate()
        {
            // The save template button should be deactivated in this state.
            // TODO: Transition to error?
            throw IntegrityCheck.Fail(
                "The save template button should be deactivated in the Idke state.");
        }

        #endregion
    }
}
