using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SimTemplate.Helpers;
using SimTemplate.Model;
using SimTemplate.Model.Database;
using SimTemplate.Model.DataControllerEventArgs;
using SimTemplate.Model.DataControllers;

namespace SimTemplate.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel
    {
        public class Loading : TransitioningAsync<GetCaptureCompleteEventArgs>
        {
            public Loading(TemplateBuilderViewModel outer) : base(outer)
            { }

            #region Overriden Abstract Methods

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Indicate that we are loading
                Outer.PromptText = "Loading capture...";
                // TODO: Find better way of managing resources. Hardcoding strings is dodge...
                Outer.StatusImage = new Uri("pack://application:,,,/Resources/StatusImages/Loading.png");
                Outer.LoadIcon = "pack://application:,,,/Resources/Icons/Cancel.ico";
            }

            public override void OnLeavingState()
            {
                base.OnLeavingState();

                // Ensure load icon is shown and status image is cleared.
                Outer.StatusImage = null;
                Outer.LoadIcon = "pack://application:,,,/Resources/Icons/Load.ico";
            }

            public override void LoadFile()
            {
                // Load file command while loading means cancel
                // TODO: this feels wrong...should probably send a different command? or at least rename?
                Outer.m_DataController.AbortRequest((Guid)Identifier);
                // User cancelled operation
                Outer.PromptText = "Capture request aborted.";
                Logger.DebugFormat(
                    "Cpture request to DataController cancelled.",
                    Outer.FilteredScannerType);
                TransitionTo(typeof(Idle));
            }

            public override void PositionInput(Point position)
            {
                // Ignore.
            }

            public override void PositionUpdate(Point position)
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
                CheckCompleteAndContinue(e.RequestId, e);
            }

            #endregion

            protected override object StartAsyncOperation()
            {
                // Request a capture from the database
                return Outer.m_DataController
                    .BeginGetCapture(Outer.FilteredScannerType, Outer.m_IsGetTemplatedCapture);
            }

            protected override void OnOperationComplete(GetCaptureCompleteEventArgs e)
            {
                // We have recieved a response from our request.
                // Indicate we are no longer loading.
                Outer.StatusImage = null;

                switch (e.Result)
                {
                    case DataRequestResult.Success:
                        IntegrityCheck.IsNotNull(e.Capture);

                        Outer.PromptText = "Capture loaded";
                        Outer.Capture = e.Capture;
                        if (Outer.Capture.TemplateData != null)
                        {
                            // If there is a template in the capture info, load it.
                            IEnumerable<MinutiaRecord> template = TemplateHelper
                                .ToMinutae(Outer.Capture.TemplateData);
                            foreach (MinutiaRecord rec in template)
                            {
                                // Ensure we use the UI thread to add to the ObservableCollection.
                                App.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    Outer.Minutae.Add(rec);
                                }));
                            }
                        }
                        TransitionTo(typeof(WaitLocation));
                        break;

                    case DataRequestResult.Failed:
                        // No capture was obtained.
                        Outer.PromptText = "No capture matching the criteria obtained.";
                        Logger.DebugFormat(
                            "Capture request returned Failed response.",
                            Outer.FilteredScannerType);
                        TransitionTo(typeof(Idle));
                        break;

                    case DataRequestResult.TaskFailed:
                        // No capture was obtained.
                        Outer.PromptText = "App failed to carry out capture request.";
                        Logger.ErrorFormat(
                            "Capture request returned TaskFailed response.",
                            Outer.FilteredScannerType);
                        TransitionTo(typeof(Idle));
                        break;

                    default:
                        throw IntegrityCheck.FailUnexpectedDefault(e.Result);
                }
            }
        }
    }
}
