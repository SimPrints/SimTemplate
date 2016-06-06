using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimTemplate.Helpers;
using SimTemplate.ViewModel;
using SimTemplate.ViewModel.Database;
using SimTemplate.ViewModel.DataControllers.EventArguments;
using SimTemplate.ViewModel.DataControllers;
using System.Windows;

namespace SimTemplate.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel
    {
        private class Saving : TransitioningAsync<SaveTemplateEventArgs>
        {
            public Saving(TemplateBuilderViewModel outer) : base(outer)
            { }

            #region Overridden Methods

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Indicate that we are loading
                Outer.PromptText = "Saving template...";
            }

            public override void PositionUpdate(Point position)
            {
                // Ignore a position change when saving.
            }

            public override void DataController_SaveTemplateComplete(SaveTemplateEventArgs e)
            {
                CheckCompleteAndContinue(e.RequestId, e);
            }

            protected override object StartAsyncOperation()
            {  
                // We should only be saving if there is information to save
                IntegrityCheck.AreNotEqual(0, Outer.Minutae.Count());

                // Convert template format.
                byte[] isoTemplate = TemplateHelper.ToIsoTemplate(Outer.Minutae);

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

            #endregion

        }
    }
}
