using log4net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SimTemplate.Helpers;
using SimTemplate.Model;
using SimTemplate.Model.Database;
using SimTemplate.StateMachine;

namespace SimTemplate.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel
    {
        public abstract class TemplateBuilderBaseState : BaseState
        {
            private ILog m_Log;

            #region Constructor

            public TemplateBuilderBaseState(TemplateBuilderViewModel outer)
                : base(outer)
            { }

            #endregion

            #region Virtual Methods

            public virtual bool IsMinutiaTypeButtonsEnabled { get { return false; } }

            public virtual void LoadFile() { MethodNotImplemented(); }

            public virtual void PositionInput(Point position) { MethodNotImplemented(); }

            public virtual void PositionUpdate(Point position) { MethodNotImplemented(); }

            public virtual void RemoveMinutia(int index) { MethodNotImplemented(); }

            public virtual void MoveMinutia(Point position) { MethodNotImplemented(); }

            public virtual void SaveTemplate() { MethodNotImplemented(); }

            public virtual void SetMinutiaType(MinutiaType type) { MethodNotImplemented(); }

            public virtual void SetScannerType(ScannerType type) { MethodNotImplemented(); }

            public virtual void EscapeAction() { MethodNotImplemented(); }

            public virtual void StartMove(int index) { MethodNotImplemented(); }

            public virtual void BeginInitialise() { MethodNotImplemented(); }
            #endregion

            #region Event Handlers

            public virtual void DataController_InitialisationComplete(
                InitialisationCompleteEventArgs e)
            {
                MethodNotImplemented();
            }

            public virtual void DataController_GetCaptureComplete(
                GetCaptureCompleteEventArgs e)
            {
                MethodNotImplemented();
            }

            public virtual void DataController_SaveTemplateComplete(
                SaveTemplateEventArgs e)
            {
                MethodNotImplemented();
            }

            #endregion

            #region Protected Members

            /// <summary>
            /// Gets the outer class that this state is behaviour for.
            /// </summary>
            protected TemplateBuilderViewModel Outer { get { return (TemplateBuilderViewModel)BaseOuter; } }

            protected ILog Log
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

            /// <summary>
            /// Transitions to the specified state.
            /// </summary>
            /// <param name="newState">The new state.</param>
            protected void TransitionTo(Type newState)
            {
                Outer.m_StateMgr.TransitionTo(newState);
            }

            /// <summary>
            /// Called when an error occurred, to report it and transition to error state.
            /// </summary>
            /// <param name="ex">The exception.</param>
            protected void OnErrorOccurred(TemplateBuilderException ex)
            {
                Outer.m_Exception = ex;
                Logger.ErrorFormat("Error occurred: " + ex.Message, ex);
                TransitionTo(typeof(Error));
            }

            #endregion
        }
    }
}
