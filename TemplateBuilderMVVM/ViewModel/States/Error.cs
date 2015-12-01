using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TemplateBuilder.ViewModel.States
{
    public class Error : State
    {
        public Error(TemplateBuilderViewModel outer, StateManager stateMgr) : base(outer, stateMgr)
        { }

        public override void EscapeAction()
        {
            // Do nothing.
        }

        public override void image_SizeChanged(Size newSize)
        {
            // Do nothing.
        }

        public override void OpenFile()
        {
            // Do nothing.
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
            // Do nothing.
        }

        public override void SaveTemplate()
        {
            // Do nothing.
        }

        public override void SetMinutiaType(MinutiaType type)
        {
            // Do nothing.
        }
    }
}
