using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model;

namespace TemplateBuilder.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel
    {
        public class WaitLocation : Templating
        {
            public WaitLocation(TemplateBuilderViewModel outer) : base(outer)
            { }

            public override void PositionInput(Point position)
            {
                // The user is starting to record a new minutia

                // Start a new minutia data record.
                MinutiaRecord record = new MinutiaRecord();

                // Save the position
                record.Position = position;
                // Save the current type.
                record.Type = Outer.InputMinutiaType;
                // Record minutia information.
                Outer.Minutae.Add(record);

                // Indicate next input defines the direction.
                TransitionTo(typeof(WaitDirection));
            }

            public override void PositionUpdate(Point e)
            {
                // Ignore.
            }

            public override void RemoveMinutia(int index)
            {
                // Remove the item at the specified index.
                Outer.Minutae.RemoveAt(index);
            }

            public override void SaveTemplate()
            {
                IntegrityCheck.IsNotNull(Outer.Capture);

                byte[] isoTemplate;
                if (Outer.Minutae.Count() > 0)
                {
                    isoTemplate = TemplateHelper.ToIsoTemplate(Outer.Minutae);
                }
                else
                {
                    // User has saved 0 minutia, save as 'NULL' in database
                    isoTemplate = null;
                }

                bool isSaved = Outer.m_DataController.SaveTemplate(
                    Outer.Capture.Guid, 
                    Outer.Capture.DbId,
                    isoTemplate);

                if (isSaved)
                {
                    // We've finished with this image, so transition to Idle state.
                    TransitionTo(typeof(Idle));
                }
                else
                {
                    // Failed to save the template successfully.
                    // TODO: show dialog to try again?
                    TransitionTo(typeof(Idle));
                }
                // TODO: Implement Heirachical state machine.
                ClearUpTemplating();
            }
            public override void EscapeAction()
            {
                // Nothing to escape.
            }

            public override void SetMinutiaType(MinutiaType type)
            {
                // Ignore. No current record to update.
            }

            public override void StartMove(int index)
            {
                Outer.m_SelectedMinutia = index;
                TransitionTo(typeof(MovingMinutia));
            }

            #region Helper Methods

            private static string ToRecord(MinutiaRecord labels)
            {
                return String.Format("{0}, {1}, {2}, {3}",
                    labels.Position.X, labels.Position.Y, labels.Angle, labels.Type);
            }

            #endregion
        }
    }
}
