using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SimTemplate.ViewModel;
using SimTemplate.ViewModel.Database;
using SimTemplate.ViewModel.DataControllers.EventArguments;

namespace SimTemplate.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel
    {
        public class Idle : TemplateBuilderBaseState
        {
            public Idle(TemplateBuilderViewModel outer) : base(outer)
            { }

            public override void LoadFile()
            {
                TransitionTo(typeof(Loading));
            }

            public override void SetScannerType(ScannerType type)
            {
                // Ignore.
            }

            public override void DataController_GetCaptureComplete(GetCaptureCompleteEventArgs e)
            {
                // Ignore.
                // We may have a race condition on Aborting a get capture request.
            }
        }
    }
}
