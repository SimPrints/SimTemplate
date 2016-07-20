using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimTemplate.Helpers;
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

            #region Overridden Methods

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Indicate that we are saving
                Outer.m_TemplatingViewModel.PromptText = "Saving template...";
            }

            public override void DataController_SaveTemplateComplete(SaveTemplateEventArgs e)
            {
                CheckCompleteAndContinue(e.RequestId, e);
            }

            protected override object StartAsyncOperation()
            {
                // Get template from TemplatingViewModel.
                byte[] isoTemplate = Outer.m_TemplatingViewModel.FinaliseTemplate();

                // Begin the save
                return Outer.m_DataController.BeginSaveTemplate(
                    Outer.m_TemplatingViewModel.Capture.DbId,
                    isoTemplate);
            }

            protected override void OnOperationComplete(SaveTemplateEventArgs e)
            {
                switch (e.Result)
                {
                    case DataRequestResult.Success:
                        Outer.m_TemplatingViewModel.PromptText = "Saved successfully";
                        break;

                    case DataRequestResult.Failed:
                        Outer.m_TemplatingViewModel.PromptText = "Server failed to save";
                        break;

                    case DataRequestResult.TaskFailed:
                        Outer.m_TemplatingViewModel.PromptText = "Application failed to save";
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
