using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model;
using TemplateBuilder.Model.Database;

namespace TemplateBuilder.ViewModel.States
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

            // Connect to SQlite.
            DataControllerConfig config = new DataControllerConfig(
                Properties.Settings.Default.SqliteDatabase,
                Properties.Settings.Default.ImagesDirectory);


            bool isSuccessful = Outer.DataController.Initialise(config);
            if (isSuccessful)
            {
                m_StateMgr.TransitionTo(typeof(Idle));
            }
            else
            {
                m_StateMgr.TransitionTo(typeof(Error));
            }
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

        #region Helper Methods


        #endregion
    }
}
