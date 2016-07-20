using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SimTemplate.ViewModels;
using SimTemplate.Model.DataControllers.EventArguments;
using SimTemplate.DataTypes.Enums;

namespace SimTemplate.ViewModels
{
    public partial class MainWindowViewModel
    {
        public class Idle : Initialised
        {
            public Idle(MainWindowViewModel outer) : base(outer)
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
