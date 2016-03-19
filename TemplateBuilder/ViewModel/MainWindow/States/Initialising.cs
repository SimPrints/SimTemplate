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
                Outer.m_InputMinutiaType = MinutiaType.Termination;
                Outer.m_FilteredScannerType = ScannerType.All;
                Outer.m_IsGetTemplatedCapture = false;

                // TODO: provide opportunity to update SQL database location.
                // TODO: provide opportunity to update image folders.

                // Initialise the DataController so that we can fetch images.
                DataControllerConfig config = new DataControllerConfig(
                    Properties.Settings.Default.SqliteDatabase,
                    Properties.Settings.Default.ImagesDirectory);

                Outer.m_DataController.BeginInitialise(config);
            }

            #region Abstract Methods

            public override void ScaleChanged(Vector newScale)
            {
                // Ignore. Uninitialised.
            }

            public override void LoadFile()
            {
                // Ignore. Uninitialised.
            }

            public override void PositionInput(Point e, MouseButton changedButton)
            {
                // Ignore. Uninitialised.
            }

            public override void PositionMove(Point e)
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
