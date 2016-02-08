using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model;

namespace TemplateBuilder.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel
    {
        public class Idle : Initialised
        {
            public Idle(TemplateBuilderViewModel outer) : base(outer)
            { }

            #region Overriden Abstract Methods

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Deactivate UI controls.
                Outer.IsSaveTemplatePermitted = false;
                Outer.IsInputMinutiaTypePermitted = false;

                // Hide old image from UI, and remove other things.
                Outer.Image = null;
                Outer.Minutae.Clear();

                LoadImage();
            }

            public override void PositionInput(Point point, MouseButton changedButton)
            {
                // Ignore.
            }

            public override void PositionMove(Point point)
            {
                // Ignore.
            }

            public override void SetMinutiaType(MinutiaType type)
            {
                // No record to update.
            }

            public override void EscapeAction()
            {
                // Nothing to escape.
            }

            public override void StartMove(int index)
            {
                // Ignore.
            }

            #endregion
        }
    }
}
