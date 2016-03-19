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
        public class Loading : Initialised
        {
            private Guid m_CaptureRequestId;

            public Loading(TemplateBuilderViewModel outer) : base(outer)
            { }

            #region Overriden Abstract Methods

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Show loading status image
                Outer.StatusImage = new Uri("pack://application:,,,/Resources/Loading.png");

                // Request a capture from the database
                m_CaptureRequestId = Outer.m_DataController
                    .BeginGetCapture(Outer.FilteredScannerType, Outer.m_IsGetTemplatedCapture);
            }

            public override void LoadFile()
            {
                // Assume user has changed settings and start load again.
                // TODO: Cancel current request and try again.
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
                        if (Outer.Capture.TemplateData != null)
                        {
                            IEnumerable<MinutiaRecord> template = TemplateHelper
                                .ToMinutae(Outer.Capture.TemplateData);
                            foreach (MinutiaRecord rec in template)
                            {
                                App.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    Outer.Minutae.Add(rec);
                                }));
                            }
                        }
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
