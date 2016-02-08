using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model;
using TemplateBuilder.Model.Database;

namespace TemplateBuilder.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel
    {
        public class Error : TemplateBuilderBaseState
        {
            public Error(TemplateBuilderViewModel outer) : base(outer)
            { }

            public override void DataController_InitialisationComplete(InitialisationCompleteEventArgs e)
            {
                throw IntegrityCheck.Fail("Not expected to have InitialisationComplete event when in error.");
            }

            public override void EscapeAction()
            {
                // Ignore.
            }

            public override void image_SizeChanged(Size newSize)
            {
                // Ignore.
            }

            public override void SkipFile()
            {
                // Ignore.
            }

            public override void PositionInput(Point point, MouseButton changedButton)
            {
                // Ignore.
            }

            public override void PositionMove(Point point)
            {
                // Ignore.
            }

            public override void RemoveMinutia(int index)
            {
                // Ignore.
            }

            public override void SaveTemplate()
            {
                // Ignore.
            }

            public override void SetMinutiaType(MinutiaType type)
            {
                // Ignore.
            }

            public override void MoveMinutia(Point point)
            {
                // Ignore.
            }

            public override void StartMove(int index)
            {
                // Ignore.
            }
        }
    }
}
