using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model;

namespace TemplateBuilder.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel
    {
        public class WaitDirection : Templating
        {
            private MinutiaRecord m_Record;

            public WaitDirection(TemplateBuilderViewModel outer) : base(outer)
            { }

            public override bool IsMinutiaTypeButtonsEnabled { get { return true; } }

            #region Overriden Public Methods

            public override void OnEnteringState()
            {
                base.OnEnteringState();
                m_Record = Outer.Minutae.Last();
                IntegrityCheck.IsNotNull(m_Record.Position);
            }

            public override void PositionUpdate(Point position)
            {
                // Update the direction whenever the mouse moves.
                SetDirection(position);
            }

            public override void PositionInput(Point position)
            {
                // The user has just finalised the direction of the minutia.
                SetDirection(position);
                TransitionTo(typeof(WaitLocation));
            }

            public override void RemoveMinutia(int index)
            {
                //Do nothing.
            }

            public override void SaveTemplate()
            {
                Logger.Debug("Cannot save template when waiting on direction.");
            }

            public override void SetMinutiaType(MinutiaType type)
            {
                // Update minutia type as user has changed it.
                m_Record.Type = Outer.InputMinutiaType;
            }

            public override void EscapeAction()
            {
                // Cancel adding the current minutia.
                Outer.Minutae.Remove(m_Record);
                TransitionTo(typeof(WaitLocation));
            }

            #endregion

            #region Private Methods

            private void SetDirection(Point p)
            {
                // Get the relevant record
                Vector direction = p - m_Record.Position;

                // Calculate the angle (in degrees)
                double angle = TemplateHelper.RadianToDegree(Math.Atan2(direction.Y, direction.X));
                
                // Save the new direction
                m_Record.Angle = angle;
            }

            public override void StartMove(int index)
            {
                // The user may click the minutia when setting direction but this shouldn't start a
                // move!
                // Ignore.
            }

            #endregion
        }
    }
}
