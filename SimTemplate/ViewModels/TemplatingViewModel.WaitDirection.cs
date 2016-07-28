using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SimTemplate.Utilities;
using SimTemplate.ViewModels;
using SimTemplate.DataTypes;
using SimTemplate.DataTypes.Enums;

namespace SimTemplate.ViewModels
{
    public partial class TemplatingViewModel
    {
        public class WaitDirection : Initialised
        {
            private MinutiaRecord m_Record;

            public WaitDirection(TemplatingViewModel outer) : base(outer)
            { }

            #region Overriden Public Methods

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                Outer.OnUserActionRequired(new UserActionRequiredEventArgs("Please set minutia angle"));

                // Get the minutia that was placed in the previous step
                IntegrityCheck.AreNotEqual(0, Outer.Minutae.Count());
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
                double angle = IsoTemplateHelper.RadianToDegree(Math.Atan2(direction.Y, direction.X));
                
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
