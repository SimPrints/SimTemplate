using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TemplateBuilderMVVM.Model;

namespace TemplateBuilderMVVM.ViewModel.States
{
    public class WaitDirection : Templating
    {
        public WaitDirection(TemplateBuilderViewModel outer, StateManager stateMgr) : base(outer, stateMgr)
        { }

        public override void PositionMove(Point pos)
        {
            // Get the relevant record
            MinutiaRecord record = m_Outer.Minutae.Last();
            Vector direction = pos - record.Location;
            direction.Normalize();
            // Save the new direction
            record.Direction = direction;
        }

        public override void PositionInput(Point pos)
        {
            // The user has just finalised the direction of the minutia
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
    }
}
