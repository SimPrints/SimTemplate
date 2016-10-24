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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimTemplate.Utilities;
using SimTemplate.ViewModels;
using System.Windows;
using SimTemplate.Model.DataControllers.EventArguments;
using SimTemplate.Model.DataControllers;
using SimTemplate.DataTypes.Enums;

namespace SimTemplate.ViewModels
{
    public partial class MainWindowViewModel
    {
        private class Saving : TransitioningAsync<SaveTemplateEventArgs>
        {
            private const string SAVING_TEXT = "Saving template...";

            public Saving(MainWindowViewModel outer)
                : base(outer, Activity.Transitioning, SAVING_TEXT)
            { }

            #region TransitioningAsync Methods

            protected override object StartAsyncOperation()
            {
                // Get template from TemplatingViewModel.
                byte[] isoTemplate = Outer.m_TemplatingViewModel.FinaliseTemplate();

                // Begin the save
                return Outer.m_DataController.BeginSaveTemplate(
                    Outer.m_TemplatingViewModel.Capture.DbId,
                    isoTemplate);
            }

            protected override void AbortAsyncOperation(object identifier)
            {
                Outer.m_DataController.AbortRequest((Guid)identifier);
            }

            #endregion

            #region Event Handlers

            public override void DataController_SaveTemplateComplete(SaveTemplateEventArgs e)
            {
                CheckCompleteAndContinue(e.RequestId, e);
            }

            protected override void OnOperationComplete(SaveTemplateEventArgs e)
            {
                switch (e.Result)
                {
                    case DataRequestResult.Success:
                        Outer.PromptText = "Saved successfully";
                        // Save operation complete
                        // Clear the TemplatingViewModel of any leftover information
                        Outer.TemplatingViewModel.QuitTemplating();
                        // Now transition to Idle
                        TransitionTo(typeof(Idle));
                        break;

                    case DataRequestResult.Failed:
                        Outer.PromptText = "Server failed to save";
                        // Save operation failed due to connection. Wait
                        TransitionTo(typeof(Templating));
                        break;

                    case DataRequestResult.TaskFailed:
                        Outer.PromptText = "Application failed to save";
                        // Save operation failed due to bug. Transition to Fault
                        TransitionTo(typeof(Error));
                        break;

                    default:
                        throw IntegrityCheck.FailUnexpectedDefault(e.Result);
                }
            }

            #endregion

        }
    }
}
