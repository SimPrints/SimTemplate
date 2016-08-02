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
            public Saving(MainWindowViewModel outer) : base(outer, Activity.Transitioning)
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
