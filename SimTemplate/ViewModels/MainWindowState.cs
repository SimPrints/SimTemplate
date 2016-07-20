using SimTemplate.DataTypes.Enums;
using SimTemplate.Helpers;
using SimTemplate.Model.DataControllers.EventArguments;
using SimTemplate.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.ViewModels
{
    public partial class MainWindowViewModel
    {
        public class MainWindowState : State
        {
            #region Constructor

            public MainWindowState(ViewModel outer) : base(outer)
            {
            }

            #endregion

            #region Virtual Methods

            public virtual void LoadFile() { MethodNotImplemented(); }

            public virtual void SaveTemplate() { MethodNotImplemented(); }

            public virtual void SetScannerType(ScannerType type) { MethodNotImplemented(); }

            public virtual void EscapeAction() { MethodNotImplemented(); }

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
            protected MainWindowViewModel Outer { get { return (MainWindowViewModel)BaseOuter; } }

            protected void TransitionTo(Type newState)
            {
                Outer.m_StateMgr.TransitionTo(newState);
            }

            /// <summary>
            /// Called when an error occurred, to report it and transition to error state.
            /// </summary>
            /// <param name="ex">The exception.</param>
            protected void OnErrorOccurred(SimTemplateException ex)
            {
                Outer.m_Exception = ex;
                Log.ErrorFormat("Error occurred: " + ex.Message, ex);
                TransitionTo(typeof(Error));
            }

            #endregion
        }
    }
}
