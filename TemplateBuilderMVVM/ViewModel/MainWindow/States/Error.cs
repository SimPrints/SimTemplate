using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model;
using TemplateBuilder.Model.Database;

namespace TemplateBuilder.ViewModel.MainWindow.States
{
    public class Error : State
    {
        public Error(TemplateBuilderViewModel outer, StateManager stateMgr) : base(outer, stateMgr)
        { }

        public override void DataController_InitialisationComplete(InitialisationCompleteEventArgs e)
        {
            throw IntegrityCheck.Fail("Not expected to have InitialisationComplete event when in error.");
        }

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
