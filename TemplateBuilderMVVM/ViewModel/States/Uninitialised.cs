using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model;

namespace TemplateBuilder.ViewModel.States
{
    public class Uninitialised : State
    {
        public Uninitialised(TemplateBuilderViewModel outer, StateManager stateMgr)
            : base(outer, stateMgr) { }

        public override void OnEnteringState()
        {
            base.OnEnteringState();

            // Initialise properties.
            m_Outer.Minutae = new TrulyObservableCollection<MinutiaRecord>();
            m_Outer.InputMinutiaType = MinutiaType.Termination;

            // Connect to SQlite.
            SQLiteConnection m_dbConnection = new SQLiteConnection(
                String.Format("Data Source={0};Version=3;", m_Outer.Parameters.SqliteDatabase));

            bool isException = false;
            try
            {
                m_dbConnection.Open();
            }
            catch (SqlException ex)
            {
                isException = true;
                OnErrorOccurred(new TemplateBuilderException(
                    "Failed to connect to SQLite database", ex));
            }

            if (!isException)
            {
                // Set the connected database to be available from ViewModel. 
                m_Outer.SQLiteConnection = m_dbConnection;
                // Transition to Initialised.
                m_StateMgr.TransitionTo(typeof(Idle));
            }
        }

        public override void image_SizeChanged(Size newSize)
        {
            // Do nothing. Uninitialised.
        }

        public override void OpenFile()
        {
            // Do nothing. Uninitialised.
        }

        public override void OpenFolder()
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
    }
}
