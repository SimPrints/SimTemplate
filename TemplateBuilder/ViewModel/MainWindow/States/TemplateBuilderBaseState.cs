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
using TemplateBuilder.Helpers;
using TemplateBuilder.Model;
using TemplateBuilder.Model.Database;
using TemplateBuilder.StateMachine;

namespace TemplateBuilder.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel
    {
        public abstract class TemplateBuilderBaseState : BaseState
        {
            #region Constructor

            public TemplateBuilderBaseState(TemplateBuilderViewModel outer)
                : base(outer)
            { }

            #endregion

            #region Virtual Methods

            public virtual bool IsMinutiaTypeButtonsEnabled { get { return false; } }

            public virtual void SkipFile() { MethodNotImplemented(); }

            public virtual void PositionInput(Point point, MouseButton changedButton) { MethodNotImplemented(); }

            public virtual void PositionMove(Point point) { MethodNotImplemented(); }

            public virtual void RemoveMinutia(int index) { MethodNotImplemented(); }

            public virtual void MoveMinutia(Point point) { MethodNotImplemented(); }

            public virtual void SaveTemplate() { MethodNotImplemented(); }

            public virtual void image_SizeChanged(Size newSize) { MethodNotImplemented(); }

            public virtual void SetMinutiaType(MinutiaType type) { MethodNotImplemented(); }

            public virtual void SetScannerType(ScannerType type) { MethodNotImplemented(); }

            public virtual void EscapeAction() { MethodNotImplemented(); }

            public virtual void StartMove(int index) { MethodNotImplemented(); }
            #endregion

            #region Event Handlers

            public virtual void DataController_InitialisationComplete(
                InitialisationCompleteEventArgs e)
            {
                MethodNotImplemented();
            }

            #endregion

            /// <summary>
            /// Gets the outer class that this state is behaviour for.
            /// </summary>
            protected TemplateBuilderViewModel Outer { get { return (TemplateBuilderViewModel)BaseOuter; } }

            /// <summary>
            /// Transitions to the specified state.
            /// </summary>\\Mac\Home\Code\SimPrint\C#\SimTemplate\TemplateBuilder\ViewModel\MainWindow\States\TemplateBuilderBaseState.cs
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
        }
    }
}
