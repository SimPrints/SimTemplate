using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SimTemplate.Utilities;
using SimTemplate.ViewModels;
using SimTemplate.Model.DataControllers.EventArguments;
using SimTemplate.Model.DataControllers;
using SimTemplate.DataTypes;
using SimTemplate.DataTypes.Enums;

namespace SimTemplate.ViewModels
{
    public partial class MainWindowViewModel
    {
        public class Loading : TransitioningAsync<GetCaptureCompleteEventArgs>
        {
            public Loading(MainWindowViewModel outer) : base(outer, Activity.Loading)
            { }

            #region Overriden Public Methods

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Indicate that we are loading
                Outer.PromptText = "Loading capture...";
                //Outer.LoadIconOverride = Properties.Resources.Load;
            }

            public override void LoadFile()
            {
                // Load file command while loading means cancel
                // TODO: this feels wrong...should probably send a different command? or at least rename?
                Outer.m_DataController.AbortRequest((Guid)Identifier);
                // User cancelled operation
                Outer.PromptText = "Capture request aborted.";
                Log.DebugFormat(
                    "Capture request to DataController cancelled.",
                    Outer.FilteredScannerType);
                TransitionTo(typeof(Idle));
            }

            public override void EscapeAction()
            {
                // Nothing to escape.
            }

            #endregion

            #region Event Handlers

            public override void DataController_GetCaptureComplete(GetCaptureCompleteEventArgs e)
            {
                CheckCompleteAndContinue(e.RequestId, e);
            }

            #endregion

            #region TransitioningAsync Methods

            protected override object StartAsyncOperation()
            {
                // Request a capture from the database
                return Outer.m_DataController
                    .BeginGetCapture(Outer.FilteredScannerType);
            }

            protected override void AbortAsyncOperation(object identifier)
            {
                Outer.m_DataController.AbortRequest((Guid)identifier);
            }

            protected override void OnOperationComplete(GetCaptureCompleteEventArgs e)
            {
                // We have recieved a response from our request.
                // Indicate we are no longer loading.

                switch (e.Result)
                {
                    case DataRequestResult.Success:
                        IntegrityCheck.IsNotNull(e.Capture);

                        Outer.PromptText = "Capture loaded";
                        Outer.m_TemplatingViewModel.BeginTemplating(e.Capture);
                        TransitionTo(typeof(Templating));
                        break;

                    case DataRequestResult.Failed:
                        // No capture was obtained.
                        Outer.PromptText = "No capture matching the criteria obtained.";
                        Log.DebugFormat(
                            "Capture request returned Failed response.",
                            Outer.FilteredScannerType);
                        TransitionTo(typeof(Idle));
                        break;

                    case DataRequestResult.TaskFailed:
                        // No capture was obtained.
                        Outer.PromptText = "App failed to carry out capture request.";
                        Log.ErrorFormat(
                            "Capture request returned TaskFailed response.",
                            Outer.FilteredScannerType);
                        TransitionTo(typeof(Idle));
                        break;

                    default:
                        throw IntegrityCheck.FailUnexpectedDefault(e.Result);
                }
            }

            #endregion
        }
    }
}
