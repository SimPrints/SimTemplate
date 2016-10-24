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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SimTemplate.Utilities;
using System;
using SimTemplate.DataTypes.Collections;
using SimTemplate.DataTypes.Enums;
using SimTemplate.Model.DataControllers.EventArguments;
using SimTemplate.Model.DataControllers;
using SimTemplate.DataTypes;
using System.Collections;
using System.Collections.Generic;

namespace SimTemplate.ViewModels
{
    public partial class MainWindowViewModel
    {
        public class Initialising : TransitioningAsync<InitialisationCompleteEventArgs>
        {
            private const string INITIALISING_TEXT = "Initialising...";

            public Initialising(MainWindowViewModel outer)
                : base(outer, Activity.Transitioning, INITIALISING_TEXT)
            { }

            #region Overriden Public Methods

            public override void OnEnteringState()
            {
                // Initialise properties.
                Outer.FilteredScannerType = ScannerType.None;

                // Initialise TemplatingViewModel
                Outer.m_TemplatingViewModel.BeginInitialise();

                // Check that our current settings are valid
                if (!Outer.m_SettingsManager.ValidateCurrentSettings())
                {
                    // If settings invalid then transition to error and prompt the user to update them
                    OnErrorOccurred(new SimTemplateException("Invalid settings"));
                }
                else
                {
                    // As valid settings are definitely provided, perform the asynchronous authentication
                    base.OnEnteringState();
                }
            }

            public override void LoadFile()
            {
                // Ignore. Uninitialised.
            }

            public override void SaveTemplate()
            {
                // Ignore. Uninitialised.
            }

            public override void EscapeAction()
            {
                // Ignore. Uninitialised.
            }

            public override void SetScannerType(ScannerType type)
            {
                // Ignore.
            }

            #endregion

            #region TransitioningAsync Methods

            protected override object StartAsyncOperation()
            {
                // Initialise the DataController so that we can fetch images.
                DataControllerConfig config = new DataControllerConfig(
                    (string)Outer.m_SettingsManager.GetCurrentSetting(Setting.ApiKey),
                    (string)Outer.m_SettingsManager.GetCurrentSetting(Setting.RootUrl));
                return Outer.m_DataController.BeginInitialise(config);
            }

            protected override void AbortAsyncOperation(object identifier)
            {
                Outer.m_DataController.AbortRequest((Guid)identifier);
            }

            #endregion

            #region Event Handlers

            public override void DataController_InitialisationComplete(InitialisationCompleteEventArgs e)
            {
                CheckCompleteAndContinue(e.RequestId, e);
            }

            protected override void OnOperationComplete(InitialisationCompleteEventArgs e)
            {
                // Make state transitions based on result.
                if (e.Result == InitialisationResult.Initialised)
                {
                    TransitionTo(typeof(Idle));
                }
                else
                {
                    // TODO: Add exception field to InitialisationComplete to get more info on failure.
                    OnErrorOccurred(new SimTemplateException("Failed to initialise DataController."));
                }
            }

            #endregion
        }
    }
}
