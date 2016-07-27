using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SimTemplate.Utilities;
using SimTemplate.ViewModels;
using SimTemplate.DataTypes;
using SimTemplate.DataTypes.Enums;

namespace SimTemplate.ViewModels
{
    public partial class TemplatingViewModel
    {
        public class WaitLocation : Initialised
        {
            #region Constructor

            public WaitLocation(TemplatingViewModel outer) : base(outer)
            { }

            #endregion

            #region Virtual Overrides

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                Outer.PromptText = "Please place minutia";
            }

            #region View Methods

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

            #endregion

            #region ITemplatingViewModel

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

            public override byte[] FinaliseTemplate()
            {
                // We should only be saving if there is information to save
                IntegrityCheck.AreNotEqual(0, Outer.Minutae.Count());

                // Return in ISO template format.
                return IsoTemplateHelper.ToIsoTemplate(Outer.Minutae);
            }

            #endregion

            #endregion

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
