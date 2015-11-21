using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TemplateBuilderMVVM.Helpers;
using TemplateBuilderMVVM.Model;

namespace TemplateBuilderMVVM.ViewModel.States
{
    public class WaitDirection : Templating
    {
        private MinutiaRecord m_Record;

        public WaitDirection(TemplateBuilderViewModel outer, StateManager stateMgr) : base(outer, stateMgr)
        { }

        public override bool IsMinutiaTypeButtonsEnabled { get { return true; } }

        #region Overriden Public Methods

        public override void OnEnteringState()
        {
            base.OnEnteringState();
            m_Record = m_Outer.Minutae.Last();
            IntegrityCheck.IsNotNull(m_Record.Location);
        }

        public override void PositionMove(Point p)
        {
            // Update the direction whenever the mouse moves.
            SetDirection(p);
        }

        public override void PositionInput(Point p)
        {
            // The user has just finalised the direction of the minutia.
            SetDirection(p);
            m_Record.Type = m_Outer.InputMinutiaType;
            m_StateMgr.TransitionTo(typeof(WaitLocation));
        }

        public override void RemoveItem(int index)
        {
            //Do nothing/
        }

        public override void SaveTemplate()
        {
            Console.WriteLine("Cannot save template when waiting on direction.");
        }

        #endregion

        #region Private Methods

        private void SetDirection(Point p)
        {
            // Get the relevant record
            Vector direction = p - m_Record.Location.Scale(m_Outer.Scale);
            double angle = Math.Atan2(direction.Y, direction.X);
            // Save the new direction
            m_Record.Direction = angle;
        }

        #endregion
    }
}
