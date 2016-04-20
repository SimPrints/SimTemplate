using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimTemplate.Helpers;
using SimTemplate.Model;
using SimTemplate.Model.Database;

namespace SimTemplate.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel
    {
        private class Saving : TransitioningAsync<SaveTemplateEventArgs>
        {
            public Saving(TemplateBuilderViewModel outer) : base(outer)
            { }

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Indicate that we are loading
                Outer.PromptText = "Saving template...";
            }

            public override void DataController_SaveTemplateComplete(SaveTemplateEventArgs e)
            {
                base.DataController_SaveTemplateComplete(e);

                CheckCompleteAndContinue(e.RequestId, e);
            }

            protected override object StartAsyncOperation()
            {
                // Convert template format.
                byte[] isoTemplate;
                if (Outer.Minutae.Count() > 0)
                {
                    isoTemplate = TemplateHelper.ToIsoTemplate(Outer.Minutae);
                }
                else
                {
                    // User has saved 0 minutia, save as 'NULL' in database
                    isoTemplate = null;
                }

                // Begin the save
                return Outer.m_DataController.BeginSaveTemplate(
                    Outer.Capture.DbId,
                    isoTemplate);
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
        }
    }
}
