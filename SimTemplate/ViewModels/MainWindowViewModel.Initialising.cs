using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SimTemplate.Helpers;
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
        public class Initialising : MainWindowState
        {
            public Initialising(MainWindowViewModel outer)
                : base(outer)
            { }

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Indicate we are loading
                //Outer.StatusImage = new BitmapImage();

                // Initialise properties.
                Outer.FilteredScannerType = ScannerType.None;

                Outer.m_TemplatingViewModel.StatusImage = new Uri("pack://application:,,,/Resources/StatusImages/Loading.png");

                // Initialise the DataController so that we can fetch images.
                DataControllerConfig config = new DataControllerConfig(
                    Properties.Settings.Default.ApiKey,
                    Properties.Settings.Default.RootUrl);
                Outer.m_DataController.BeginInitialise(config);

                // Initialise the TemplatingViewModel so that we can process images.
                Outer.m_TemplatingViewModel.BeginInitialise();
            }

            public override void OnLeavingState()
            {
                Outer.m_TemplatingViewModel.StatusImage = null;
            }

            #region Abstract Methods

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

            #region Event Handlers

            public override void DataController_InitialisationComplete(InitialisationCompleteEventArgs e)
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
