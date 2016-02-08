using System.Windows;
using System.Windows.Input;
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

                // Initialise properties.
                Outer.Minutae = new TrulyObservableCollection<MinutiaRecord>();
                Outer.InputMinutiaType = MinutiaType.Termination;

                // TODO: provide opportunity to update SQL database location.
                // TODO: provide opportunity to update image folders.

                // Initialise the DataController so that we can fetch images.
                DataControllerConfig config = new DataControllerConfig(
                    Properties.Settings.Default.SqliteDatabase,
                    Properties.Settings.Default.ImagesDirectory);

                Outer.m_DataController.Initialise(config);
            }

            #region Abstract Methods

            public override void image_SizeChanged(Size newSize)
            {
                // Ignore. Uninitialised.
            }

            public override void SkipFile()
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

            #endregion

            #region Event Handlers

            public override void DataController_InitialisationComplete(InitialisationCompleteEventArgs e)
            {
                // Make state transitions based on result.
                if (e.IsSuccessful)
                {
                    TransitionTo(typeof(Idle));
                }
                else
                {
                    OnErrorOccurred(
                        new TemplateBuilderException("Failed to initialise DataController."));
                }
            }

            #endregion

            #region Event Handlers

            #endregion
        }
    }
}
