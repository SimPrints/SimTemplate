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

            DataControllerConfig config = new DataControllerConfig(
                Properties.Settings.Default.SqliteDatabase,
                Properties.Settings.Default.ImagesDirectory);

            // The Progress<T> constructor captures our UI context,
            //  so the lambda will be run on the UI thread.
            //var progress = new Progress<int>(percent =>
            //{
            //    Logger.DebugFormat("Initialisation: {0}", percent);
            //});
            Outer.OnProgressChanged(new ProgressChangedEventArgs(0, DialogAction.Show));
            Outer.DataController.Initialise(config, new Progress<int>(Progress_ReportProgress));
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

        public override void RemoveItem(int index)
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

        #endregion

        #region Event Handlers

        public override void DataController_InitialisationComplete(InitialisationCompleteEventArgs e)
        {   
            // Notify subscribers that initialisation is complete.
            Outer.OnProgressChanged(
                new ProgressChangedEventArgs(100, DialogAction.Hide));

            // Make state transitions based on result.
            if (e.IsSuccessful)
            {
                m_StateMgr.TransitionTo(typeof(Idle));
            }
            else
            {
                m_StateMgr.TransitionTo(typeof(Error));
            }
        }

        #endregion

        #region Event Handlers
        
        private void Progress_ReportProgress(int value)
        {
            Logger.DebugFormat("Progress_ReportProgress(value={0}) called.", value);
            Outer.OnProgressChanged(
                new ProgressChangedEventArgs(value, DialogAction.DoNothing));
        }

        #endregion
    }
}
