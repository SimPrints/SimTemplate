using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SimTemplate.Helpers;
using SimTemplate.Model;
using SimTemplate.Model.Database;
using SimTemplate.Model.DataControllers.EventArguments;
using SimTemplate.Model.DataControllers;

namespace SimTemplate.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel
    {
        public class Initialising : TemplateBuilderBaseState
        {
            public Initialising(TemplateBuilderViewModel outer)
                : base(outer)
            { }

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Indicate we are loading
                //Outer.StatusImage = new BitmapImage();

                // Initialise properties.
                Outer.Minutae = new TrulyObservableCollection<MinutiaRecord>();
                Outer.InputMinutiaType = MinutiaType.Termination;
                Outer.FilteredScannerType = ScannerType.None;
                Outer.IsGetTemplatedCapture = false;
                Outer.LoadIcon = "pack://application:,,,/Resources/Icons/Load.ico";

                // TODO: provide opportunity to update SQL database location.
                // TODO: provide opportunity to update image folders.

                // Initialise the DataController so that we can fetch images.
                DataControllerConfig config = new DataControllerConfig(
                    Properties.Settings.Default.ApiKey,
                    Properties.Settings.Default.RootUrl);

                Outer.m_DataController.BeginInitialise(config);
            }

            #region Abstract Methods

            public override void LoadFile()
            {
                // Ignore. Uninitialised.
            }

            public override void PositionInput(Point position)
            {
                // Ignore. Uninitialised.
            }

            public override void PositionUpdate(Point position)
            {
                // Ignore. Uninitialised.
            }

            public override void RemoveMinutia(int index)
            {
                // Ignore. Uninitialised.
            }

            public override void SaveTemplate()
            {
                // Ignore. Uninitialised.
            }

            public override void SetMinutiaType(MinutiaType type)
            {
                // Ignore. Uninitialised.
            }

            public override void EscapeAction()
            {
                // Ignore. Uninitialised.
            }

            public override void StartMove(int index)
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
                    OnErrorOccurred(new TemplateBuilderException("Failed to initialise DataController."));
                }
            }

            #endregion
        }
    }
}
