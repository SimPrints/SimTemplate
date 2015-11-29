using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using TemplateBuilder.Helpers;

namespace TemplateBuilder.ViewModel.States
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
            Outer.IsSaveTemplatePermitted = false;
            Outer.IsInputMinutiaTypePermitted = false;

            // Hide old image from UI, and remove other things.
            Outer.Image = null;
            Outer.Minutae.Clear();

            // Try to get a file from the database
            string imageFilename = Outer.DataController.GetImageFile();

            if (imageFilename != null)
            {
                // A file was found.
                Logger.DebugFormat("An image file was found for image: {0}.", imageFilename);
                Outer.Image = new BitmapImage(new Uri(imageFilename, UriKind.Absolute));
                m_StateMgr.TransitionTo(typeof(WaitLocation));
            }
            else
            {
                // No file was found.
                Logger.DebugFormat("No image file was found. Remaining in Idle", imageFilename);
                //m_StateMgr.TransitionTo(typeof(U))
            }
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

        public override void SetMinutiaType(MinutiaType type)
        {
            // No record to update.
        }

        public override void EscapeAction()
        {
            // Nothing to escape.
        }

        #endregion
    }
}
