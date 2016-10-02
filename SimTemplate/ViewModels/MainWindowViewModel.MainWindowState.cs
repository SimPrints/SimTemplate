// Copyright 2016 Sam Briggs
//
// This file is part of SimTemplate.
//
// SimTemplate is free software: you can redistribute it and/or modify it under the
// terms of the GNU General Public License as published by the Free Software 
// Foundation, either version 3 of the License, or (at your option) any later
// version.
//
// SimTemplate is distributed in the hope that it will be useful, but WITHOUT ANY 
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
// A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// SimTemplate. If not, see http://www.gnu.org/licenses/.
//
using SimTemplate.DataTypes.Enums;
using SimTemplate.Utilities;
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

            public virtual void SettingsViewModel_SettingsUpdated(string apiKey)
            {
                // We always want to reinitialise if the ApiKey is changed
                // ReInitialise
                if (Outer.IsTemplating)
                {
                    Outer.m_TemplatingViewModel.QuitTemplating();
                }
                TransitionTo(typeof(Initialising));
            }

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
