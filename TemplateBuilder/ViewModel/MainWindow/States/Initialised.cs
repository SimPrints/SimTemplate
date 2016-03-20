using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model.Database;

namespace TemplateBuilder.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel
    {
        abstract public class Initialised : TemplateBuilderBaseState
        {
            #region Constants

            private const int MAX_INVALID_FILES = 10;

            #endregion

            public Initialised(TemplateBuilderViewModel outer) : base(outer)
            { }

            #region Overriden Methods

            public override void DataController_InitialisationComplete(InitialisationCompleteEventArgs e)
            {
                throw IntegrityCheck.Fail("Not expected to have InitialisationComplete event when initialised.");
            }

            #endregion

            protected void ClearUpTemplating()
            {
                // Deactivate UI controls.
                Outer.IsSaveTemplatePermitted = false;
                Outer.IsTemplating = false;

                // Hide old image from UI, and remove other things.
                Outer.Capture = null;
                Outer.Minutae.Clear();
            }
        }
    }
}
