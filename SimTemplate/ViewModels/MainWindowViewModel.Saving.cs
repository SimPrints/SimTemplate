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

namespace SimTemplate.ViewModels
{
    public partial class MainWindowViewModel
    {
        private class Saving : TransitioningAsync<SaveTemplateEventArgs>
        {
            public Saving(MainWindowViewModel outer) : base(outer)
            { }

            #region Overridden Public Methods

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Indicate that we are saving
                Outer.PromptText = "Saving template...";
            }

            #endregion

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
                        break;

                    case DataRequestResult.Failed:
                        Outer.PromptText = "Server failed to save";
                        break;

                    case DataRequestResult.TaskFailed:
                        Outer.PromptText = "Application failed to save";
                        break;

                    default:
                        throw IntegrityCheck.FailUnexpectedDefault(e.Result);
                }

                // Save operation complete. Transition to Idle
                TransitionTo(typeof(Idle));
            }

            #endregion

        }
    }
}
