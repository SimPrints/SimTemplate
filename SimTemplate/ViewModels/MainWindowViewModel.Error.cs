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
using System;
using System.Windows;
using SimTemplate.Utilities;
using SimTemplate.Model.DataControllers.EventArguments;
using SimTemplate.DataTypes.Enums;

namespace SimTemplate.ViewModels
{
    public partial class MainWindowViewModel
    {
        public class Error : MainWindowState
        {
            public Error(MainWindowViewModel outer) : base(outer, Activity.Fault)
            { }

            #region Overriden Methods

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Indicate we have errored
                Outer.PromptText = Outer.m_Exception.Message;
            }

            public override void OnLeavingState()
            {
                base.OnLeavingState();

                // Clear the exception on leaving the state
                Outer.m_Exception = null;
            }

            public override void BeginInitialise()
            {
                TransitionTo(typeof(Initialising));
            }

            public override void DataController_InitialisationComplete(InitialisationCompleteEventArgs e)
            {
                throw IntegrityCheck.Fail(
                    "Not expected to have DataController.InitialisationComplete event when in error.");
            }

            public override void EscapeAction()
            {
                // Ignore.
            }

            public override void LoadFile()
            {
                // Ignore.
            }

            public override void SaveTemplate()
            {
                // Ignore.
            }

            public override void SetScannerType(ScannerType type)
            {
                // Ignore.
            }

            #endregion
        }
    }
}
