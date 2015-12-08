using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model;
using TemplateBuilder.Model.Database;
using TemplateBuilder.View;
using TemplateBuilder.ViewModel.MainWindow;

namespace TemplateBuilder.ViewModel.MainWindow.States
{
    public class Initialising : State
    {
        #region Constants



        #endregion

        public Initialising(TemplateBuilderViewModel outer, StateManager stateMgr)
            : base(outer, stateMgr) { }

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

            Outer.DataController.Initialise(config);
        }

        #region Abstract Methods

        public override void image_SizeChanged(Size newSize)
        {
            // Do nothing. Uninitialised.
        }

        public override void OpenFile()
        {
            // Do nothing. Uninitialised.
        }

        public override void PositionInput(Point e)
        {
            // Do nothing. Uninitialised.
        }

        public override void PositionMove(Point e)
        {
            // Do nothing. Uninitialised.
        }

        public override void RemoveMinutia(int index)
        {
            // Do nothing. Uninitialised.
        }

        public override void SaveTemplate()
        {
            // Do nothing. Uninitialised.
        }

        public override void SetMinutiaType(MinutiaType type)
        {
            // Do nothing. Uninitialised.
        }

        public override void EscapeAction()
        {
            // Do nothing. Uninitialised.
        }

        public override void MoveMinutia(int index, Point point)
        {
            // Do nothing. Uninitialised.
        }

        public override void StartMove()
        {
            // Do nothing. Uninitialised.
        }

        public override void StopMove()
        {
            // Do nothing. Uninitialised.
        }

        #endregion

        #region Event Handlers

        public override void DataController_InitialisationComplete(InitialisationCompleteEventArgs e)
        {
            // Make state transitions based on result.
            if (e.IsSuccessful)
            {
                StateMgr.TransitionTo(typeof(Idle));
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
