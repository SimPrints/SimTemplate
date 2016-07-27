using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SimTemplate.Utilities;
using System;
using SimTemplate.DataTypes.Collections;
using SimTemplate.DataTypes.Enums;
using SimTemplate.Model.DataControllers.EventArguments;
using SimTemplate.Model.DataControllers;
using SimTemplate.DataTypes;

namespace SimTemplate.ViewModels
{
    public partial class MainWindowViewModel
    {
        public class Initialising : TransitioningAsync<InitialisationCompleteEventArgs>
        {
            public Initialising(MainWindowViewModel outer)
                : base(outer)
            { }

            #region Overriden Public Methods

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Initialise properties.
                Outer.FilteredScannerType = ScannerType.None;

                // Initialise the TemplatingViewModel so that we can process images.
                Outer.m_TemplatingViewModel.BeginInitialise();

                // Indicate that we are initialising
                Outer.m_TemplatingViewModel.PromptText = "Initialising...";
                Outer.m_TemplatingViewModel.StatusImage = new Uri("pack://application:,,,/Resources/StatusImages/Loading.png");
            }

            public override void OnLeavingState()
            {
                Outer.m_TemplatingViewModel.StatusImage = null;
            }

            public override void LoadFile()
            {
                // Ignore. Uninitialised.
            }

            public override void SaveTemplate()
            {
                // Ignore. Uninitialised.
            }

            public override void EscapeAction()
            {
                // Ignore. Uninitialised.
            }

            public override void SetScannerType(ScannerType type)
            {
                // Ignore.
            }

            #endregion

            #region TransitioningAsync Methods

            protected override object StartAsyncOperation()
            {
                // Initialise the DataController so that we can fetch images.
                DataControllerConfig config = new DataControllerConfig(
                    Properties.Settings.Default.ApiKey,
                    Properties.Settings.Default.RootUrl);
                return Outer.m_DataController.BeginInitialise(config);
            }

            protected override void AbortAsyncOperation(object identifier)
            {
                Outer.m_DataController.AbortRequest((Guid)identifier);
            }

            #endregion

            #region Event Handlers

            public override void DataController_InitialisationComplete(InitialisationCompleteEventArgs e)
            {
                CheckCompleteAndContinue(e.RequestId, e);
            }

            protected override void OnOperationComplete(InitialisationCompleteEventArgs e)
            {
                // Make state transitions based on result.
                if (e.Result == InitialisationResult.Initialised)
                {
                    TransitionTo(typeof(Idle));
                }
                else
                {
                    OnErrorOccurred(new SimTemplateException("Failed to initialise DataController."));
                }
            }

            #endregion
        }
    }
}
