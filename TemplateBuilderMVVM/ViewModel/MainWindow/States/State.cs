using log4net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model;
using TemplateBuilder.Model.Database;

namespace TemplateBuilder.ViewModel.MainWindow.States
{
    public abstract class State
    {
        private ILog m_Log;

        private TemplateBuilderViewModel m_Outer;
        private StateManager m_StateMgr;

        #region Constructor

        public State(TemplateBuilderViewModel outer, StateManager stateMgr)
        {
            m_Outer = outer;
            m_StateMgr = stateMgr;
        }

        #endregion

        #region Virtual Methods

        public virtual void OnEnteringState() { }

        public virtual void OnLeavingState() { }

        public virtual bool IsMinutiaTypeButtonsEnabled { get { return false; } }

        #endregion

        #region Abstract Methods

        public abstract void OpenFile();

        public abstract void PositionInput(Point point);

        public abstract void PositionMove(Point point);

        public abstract void RemoveMinutia(int index);

        public abstract void MoveMinutia(int index, Point point);

        public abstract void SaveTemplate();

        public abstract void image_SizeChanged(Size newSize);

        public abstract void SetMinutiaType(MinutiaType type);

        public abstract void EscapeAction();

        public abstract void StartMove();

        public abstract void StopMove();

        #endregion

        #region Event Handlers

        public abstract void DataController_InitialisationComplete(
            InitialisationCompleteEventArgs e);

        #endregion

        public string Name { get { return GetType().Name; } }

        /// <summary>
        /// Gets the outer class that this state is behaviour for.
        /// </summary>
        protected TemplateBuilderViewModel Outer { get { return m_Outer; } }

        /// <summary>
        /// Gets the state manager.
        /// </summary>
        protected StateManager StateMgr { get { return m_StateMgr; } }

        /// <summary>
        /// Called when an error occurred, to report it and transition to error state.
        /// </summary>
        /// <param name="ex">The exception.</param>
        protected void OnErrorOccurred(TemplateBuilderException ex)
        {
            m_Outer.Exception = ex;
            Logger.ErrorFormat("Error occurred: " + ex.Message, ex);
            m_StateMgr.TransitionTo(typeof(Error));
        }

        protected ILog Logger
        {
            get
            {
                if (m_Log == null)
                {
                    m_Log = LogManager.GetLogger(this.GetType());
                }
                return m_Log;
            }
        }
    }
}
