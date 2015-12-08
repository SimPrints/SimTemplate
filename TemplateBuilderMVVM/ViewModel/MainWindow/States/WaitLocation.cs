using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model;

namespace TemplateBuilder.ViewModel.MainWindow.States
{
    class WaitLocation : Templating
    {
        public WaitLocation(TemplateBuilderViewModel outer, StateManager stateMgr) : base(outer, stateMgr)
        { }

        public override void PositionInput(Point pos)
        {
            // The user is starting to record a new minutia

            // Start a new minutia data record.
            MinutiaRecord record = new MinutiaRecord();

            // Save the position TO SCALE.
            record.Location = pos.InvScale(Outer.Scale);
            // Save the current type.
            record.Type = Outer.InputMinutiaType;
            // Record minutia information.
            Outer.Minutae.Add(record);

            // Indicate next input defines the direction.
            StateMgr.TransitionTo(typeof(WaitDirection));
        }

        public override void PositionMove(Point e)
        {
            // Do nothing.
        }

        public override void RemoveMinutia(int index)
        {
            // Remove the item at the specified index.
            Outer.Minutae.RemoveAt(index);
        }

        public override void SaveTemplate()
        {
            IntegrityCheck.IsNotNull(Outer.Image);

            byte[] isoTemplate = TemplateHelper.ToIsoTemplate(Outer.Minutae);

            bool isSaved = Outer.DataController.SaveTemplate(isoTemplate);

            if (isSaved)
            {
                // We've finished with this image, so transition to Idle state.
                StateMgr.TransitionTo(typeof(Idle));
            }
            else
            {
                // Failed to save the template successfully.
                // TODO: show dialog to try again?
                StateMgr.TransitionTo(typeof(Idle));
            }
        }
        public override void EscapeAction()
        {
            // Nothing to escape.
        }

        public override void SetMinutiaType(MinutiaType type)
        {
            // Do nothing. No current record to update.
        }

        public override void MoveMinutia(int index, Point point)
        {
            throw IntegrityCheck.Fail("Unexpected MoveMinutia(...) call in {0} state.", GetType().Name);
        }

        public override void StartMove()
        {
            StateMgr.TransitionTo(typeof(MovingMinutia));
        }

        public override void StopMove()
        {
            throw IntegrityCheck.Fail("Unexpected StopMove() call in {0} state.", GetType().Name);
        }

        #region Helper Methods

        private static string ToRecord(MinutiaRecord labels)
        {
            return String.Format("{0}, {1}, {2}, {3}",
                labels.Location.X, labels.Location.Y, labels.Direction, labels.Type);
        }

        #endregion
    }
}
