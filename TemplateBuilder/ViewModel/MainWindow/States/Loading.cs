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
using TemplateBuilder.Model.Database;

namespace TemplateBuilder.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel
    {
        // TODO: Rename to 'LoadingCapture'
        public class Loading : Initialised
        {
            private Guid m_CaptureRequestId;

            public Loading(TemplateBuilderViewModel outer) : base(outer)
            { }

            #region Overriden Abstract Methods

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Deactivate UI controls.
                Outer.IsSaveTemplatePermitted = false;
                Outer.IsInputMinutiaTypePermitted = false;

                // Hide old image from UI, and remove other things.
                Outer.Capture = null;
                Outer.Minutae.Clear();

                // Show loading status image
                Outer.StatusImage = new Uri("pack://application:,,,/Resources/Loading.png");

                // Request a capture from the database
                m_CaptureRequestId = Outer.m_DataController
                    .BeginGetCapture(Outer.FilteredScannerType, false);
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

            #region Event Handlers

            public override void DataController_GetCaptureComplete(GetCaptureCompleteEventArgs e)
            {
                if (e.RequestGuid == m_CaptureRequestId)
                {
                    // We have recieved a response from our request.
                    Outer.StatusImage = null;

                    if (e.Capture != null)
                    {
                        Outer.Capture = e.Capture;
                        TransitionTo(typeof(WaitLocation));
                    }
                    else
                    {
                        // No capture was obtained.
                        Logger.DebugFormat(
                            "Failed to obtain capture from DataController",
                            Outer.FilteredScannerType);

                        TransitionTo(typeof(Error));
                    }
                }
            }

            #endregion
        }
    }
}
